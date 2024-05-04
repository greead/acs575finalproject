# Imports
import os
import json
from typing import Coroutine, Dict, List, Type
from typing_extensions import Annotated, Doc
from fastapi import APIRouter, FastAPI, Request, Response
from fastapi.datastructures import Default
from fastapi.params import Depends
from fastapi.utils import generate_unique_id
from pydantic import BaseModel
from contextlib import asynccontextmanager
import psycopg as pg
from psycopg.rows import dict_row
import psycopg_pool as pgpl
from psycopg.types.composite import CompositeInfo, register_composite


# Connection info
PASSWORD = "***"
HOST, PORT, DBNAME, USER = "localhost", "5432", "postgres", "postgres"

def get_conn_info():
    return f"host={HOST} port={PORT} dbname={DBNAME} user={USER} password={PASSWORD}"
   
# Async context manager for the API
@asynccontextmanager
async def lifespan(app: FastAPI):
    app.async_pool = pgpl.AsyncConnectionPool(conninfo=get_conn_info())
    yield
    await app.async_pool.close()

# App entry point
app = FastAPI(lifespan=lifespan)

###
### Account CRUD operations
###
# Account model
class Account(BaseModel):
    """Account model for operations"""
    email: str
    password: str | None = None
    status: str | None = None

# Create account
@app.post("/account/new")
async def create_account(request: Request, account:Account):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    INSERT INTO account (email, password, status) VALUES (%s, %s, %s)
                """, (account.email, account.password, account.status))
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)
            else:
                return "OPERATION SUCCESSFUL"

# Simple auth
@app.get("/account/login")
async def login_account(request: Request, account:Account):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    SELECT * FROM account WHERE email = %s and password = %s
                """, (account.email, account.password))
                results = await cur.fetchall()
                if len(results) > 0:
                    return True
                else: 
                    return False
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)

# Get account status
@app.get("/account/status")
async def get_account_status(request: Request, account:Account):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    SELECT status FROM account WHERE email = %s
                """, (account.email,))
                results = await cur.fetchone()
                return results[0]
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)


# Update account status
@app.post("/account/status")
async def set_account_status(request: Request, account:Account):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    UPDATE account SET status = %s WHERE email = %s
                """, (account.status, account.email))
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)
            else:
                return "OPERATION SUCCESSFUL"
            
# Get all account characters 
@app.get("/account/characters")
async def get_all_account_characters(request: Request, account:Account):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor(row_factory=dict_row) as cur:
            try:
                await cur.execute("""
                    SELECT character.* FROM account JOIN character ON email=account_email WHERE email = %s
                """, (account.email,))
                results = await cur.fetchall()
                return results
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)
            
###
### Server CRUD operations
###
# Server model
class Server(BaseModel):
    """Server model for operations"""
    id: int
    name: str | None = None
    status: str | None = None

# Read server status
@app.get("/server/status")
async def get_server_status(request: Request, server:Server):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    SELECT status FROM server WHERE id = %s
                """, (server.id, ))
                results = await cur.fetchone()
                return results[0]
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)
            
# Update server status
@app.post("/server/status")
async def set_server_status(request: Request, server: Server):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    UPDATE server SET status = %s WHERE id = %s
                """, (server.status, server.id))
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)
            else:
                return "OPERATION SUCCESSFUL"
            
# Get all server characters 
@app.get("/server/characters")
async def get_all_account_characters(request: Request, server: Server):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor(row_factory=dict_row) as cur:
            try:
                await cur.execute("""
                    SELECT character.* FROM server JOIN character ON id=server_id WHERE id = %s
                """, (server.id,))
                results = await cur.fetchall()
                return results
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)

###
### Character CRUD operations
###
class Character(BaseModel):
    """Partial character model for operations"""
    server_id: int
    name: str
    account_email: str


# Create character
@app.post("/character/new")
async def create_character(request: Request, character: Character):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    INSERT INTO character 
                    VALUES (%s, %s, %s, ARRAY[]::item_stack_type[], ARRAY[]::item_stack_type[], 
                                  ROW(NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL), 0);
                """, (character.server_id, character.name, character.account_email))
            except pg.Error as e:
                return str(e)
            else:
                return "OPERATION SUCCESSFUL"

# Delete character
@app.delete("/character/delete")
async def delete_character(request: Request, character: Character):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:
            try:
                await cur.execute("""
                    DELETE FROM character WHERE server_id=%s and name=%s and account_email=%s
                """, (character.server_id, character.name, character.account_email))
            except pg.Error as e:
                return f"{pg.errors.lookup(e.sqlstate)}: {e.sqlstate}"
            else:
                return "OPERATION SUCCESSFUL"

###
### Character inventory CRUD operations
###
class CharacterID(BaseModel):
    """ID-only Character model for operations"""
    server_id: int
    name: str

# Get full character inventory
@app.get("/inventory/all")
async def get_inventory(request: Request, character: CharacterID):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor(row_factory=dict_row) as cur:
            try:
                await cur.execute("""
                    SELECT UNNEST(inventory) as values FROM character WHERE server_id=%s and name=%s
                """, (character.server_id, character.name))
                results = await cur.fetchall()
                return results
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)

class CharacterInventory(BaseModel):
    """Inventory character model for operations"""
    server_id: int
    name: str
    inventory: list[tuple]

@app.post("/inventory/all")
async def set_inventory(request: Request, inventory: CharacterInventory):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:            
            try:
                await cur.execute("""
                    UPDATE character SET inventory = %s::item_stack_type[] WHERE server_id=%s and name=%s
                """, (inventory.inventory, inventory.server_id, inventory.name))
            except pg.Error as e:
                return str(e)
            else:
                return "OPERATION SUCCESSFUL"

###
### Item CRUD operations
###
class Item(BaseModel):
    id: int | None = None
    name: str | None = None
    rarity: str | None = None
    description: str | None = None
    cost: int | None = None
    is_stackable: bool | None = None
    icon: str | None = None

# Create new item
@app.post("/item")
async def create_item(request: Request, item: Item):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor() as cur:            
            try:
                await cur.execute("""
                    INSERT INTO item (name, rarity, description, cost, is_stackable, icon) 
                    VALUES (%s, %s, %s, %s, %s, NULL)
                """, (item.name, item.rarity, item.description, item.cost, item.is_stackable))
            except pg.Error as e:
                return str(e)
            else:
                return "OPERATION SUCCESSFUL"
            
# Create specialized item
# @app.post("/item/{item_type}")
# async def create_specialty_item(request: Request, item: Item, item_type:str):
#      conn: pg.Connection
#      async with request.app.async_pool.connection() as conn:
#         cur: pg.Cursor
#         async with conn.cursor() as cur:            
#             try:
#                 await cur.execute("""
#                     INSERT INTO %s (name, rarity, description, cost, is_stackable, icon, defense) 
#                     VALUES (%s, %s, %s, %s, %s, NULL, 0)
#                 """, (item_type, item.name, item.rarity, item.description, item.cost, item.is_stackable))
#             except pg.Error as e:
#                 return str(e)
#             else:
#                 return "OPERATION SUCCESSFUL"

# Get item values
@app.get("/item")
async def get_item(request: Request, item: Item):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor(row_factory=dict_row) as cur:
            try:
                await cur.execute("""
                    SELECT * FROM item WHERE id=%s
                """, (item.id,))
                results = await cur.fetchall()
                return results
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)
            
@app.get("/item/all")
async def get_item(request: Request):
     conn: pg.Connection
     async with request.app.async_pool.connection() as conn:
        cur: pg.Cursor
        async with conn.cursor(row_factory=dict_row) as cur:
            try:
                await cur.execute("""
                    SELECT * FROM item
                """, ())
                results = await cur.fetchall()
                return results
            except pg.Error as e:
                return pg.errors.lookup(e.sqlstate)

# class ItemLookup(BaseModel):
#     type: str

# Get specialty item values
# @app.get("/item/specialty")
# async def get_items(request: Request, item: ItemLookup):
#      conn: pg.Connection
#      async with request.app.async_pool.connection() as conn:
#         cur: pg.Cursor
#         async with conn.cursor(row_factory=dict_row) as cur:
#             try:
#                 await cur.execute("""
#                     SELECT * FROM armor
#                 """, ())
#                 results = await cur.fetchall()
#                 return results
#             except pg.Error as e:
#                 return pg.errors.lookup(e.sqlstate)


