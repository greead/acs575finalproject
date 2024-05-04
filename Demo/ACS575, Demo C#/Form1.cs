using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace ACS575__Demo_C_ {
    public partial class Form1 : Form {

        // HTTP Client for API connections
        static HttpClient client = new HttpClient();

        
        public Form1() {
            /// Default initialization
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e) {
            /// On loading the form, clear the items list and fill it with the list of all items
            lstItems.Items.Clear();

            /// Query, get the response, and parse the response
            using HttpResponseMessage response = await client.GetAsync("http://localhost:8000/item/all");
            var respStr = await response.Content.ReadAsStringAsync();
            JsonNode? respJson = JsonNode.Parse(respStr);

            /// Loop over the parsed response, placing each item in the item list
            foreach ( var item in respJson.AsArray() ) {
                Item next = new Item(item["id"].ToString(), item["name"].ToString());
                lstItems.Items.Add(next);
            }
        }

        private void btnAddInventory_Click(object sender, EventArgs e) {
            /// Copy the selected items in the items list into the character inventory
            var items = lstItems.SelectedItems;
            foreach (var item in items) {
                lstInventory.Items.Add(item);
            }
        }

        private void btnRemInventory_Click(object sender, EventArgs e) {
            /// Remove the selected items from the character's inventory
            var indices = lstInventory.SelectedIndices;
            foreach (int index in indices) {
                lstInventory.Items.RemoveAt(index);
            }
        }

        public class ItemCredentials {
            /// <summary>
            /// Basic DTO to ease JSONifying an item request
            /// </summary>
            public int id { get; set; }
        }

        public class Item {
            /// <summary>
            /// Basic DTO to ease movement of items within the client application
            /// </summary>
            public string id { get; set; }
            public string name { get; set; }

            public Item(string id, string name) {
                /// Constructor
                this.id = id;
                this.name = name;
            }

            public async Task GetName(int id) {
                /// Request the name for the given id
                /// This method should be obsolete in future revisions
                
                // Create the request using the ItemCredentials
                var request = new HttpRequestMessage {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://localhost:8000/item"),
                    Content = new StringContent(
                    JsonSerializer.Serialize(new ItemCredentials { id = id }),
                    Encoding.UTF8,
                    "application/json"),
                };

                // Get a response, read the response, then parse it
                using HttpResponseMessage response = await client.SendAsync(request);
                var respStr = await response.Content.ReadAsStringAsync();
                JsonNode respJson = JsonNode.Parse(respStr);

                // Set the DTO's name to reflect the retrieved name
                this.name = respJson[0]["name"].ToString();
            }

            public override string ToString() {
                /// ToString representation for list view
                return name;
            }
        }

        public class InventoryCredentials {
            /// <summary>
            /// Basic DTO for sending inventory requests
            /// </summary>
            public string server_id { get; set; }
            public string name { get; set; }
        }

        private async void lstCharacters_SelectedIndexChanged(object sender, EventArgs e) {
            /// On selecting a character in a list, retrieve that character's inventory and populate the inventory list
            
            // Clear the inventory list
            lstInventory.Items.Clear();

            // Get the currently selected character
            CharacterItem? selected = lstCharacters.SelectedItem as CharacterItem;

            // Generate a request
            var request = new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:8000/inventory/all"),
                Content = new StringContent(
                    JsonSerializer.Serialize(new InventoryCredentials { server_id=selected.server_id, name=selected.name }),
                    Encoding.UTF8,
                    "application/json"),
            };

            // Get the response and parse it
            using HttpResponseMessage response = await client.SendAsync(request);
            var respStr = await response.Content.ReadAsStringAsync();
            JsonNode? respJson = JsonNode.Parse(respStr);

            // Add each inventory item to the inventory list
            foreach (var item in respJson.AsArray()) {
                Item next = new Item("1", "temp");
                await next.GetName(int.Parse(next.id));
                lstInventory.Items.Add(next);
            }

        }

        public class InvSaveCred {
            /// <summary>
            /// Basic DTO for sending Inventory save requests
            /// </summary>
            public int server_id { get; set; }
            public string name { get; set; }
            public List<List<int>>? inventory { get; set; }
        }

        private async void btnSaveInv_Click(object sender, EventArgs e) {
            /// Save the current inventory to the currently selected character
            
            // Create an empty inventory list
            List<List<int>> inventory = new List<List<int>>();

            // Get the currently selected character
            CharacterItem? credentials = lstCharacters.SelectedItem as CharacterItem;

            // Compile each inventory item in the current list
            foreach (Item it in lstInventory.Items) {
                inventory.Add(new List<int>([int.Parse(it.id), 1]));
            }

            // Develop an HTTP request
            var request = new HttpRequestMessage {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:8000/inventory/all"),
                Content = new StringContent(
                    JsonSerializer.Serialize(new InvSaveCred { name=credentials.name, server_id=int.Parse(credentials.server_id), inventory=inventory}),
                    Encoding.UTF8,
                    "application/json"),
            };

            // Send the request, get the response, and parse it (unused)
            using HttpResponseMessage response = await client.SendAsync(request);
            var respStr = await response.Content.ReadAsStringAsync();
            JsonNode? respJson = JsonNode.Parse(respStr);

        }

        public class CharacterCredentials {
            /// <summary>
            /// Basic DTO for making Character requests
            /// </summary>
            public string email { get; set;}
        }

        public class CharacterItem {
            /// <summary>
            /// Basic DTO for transferring character related information throughout the application
            /// </summary>
            public string name { get; set; }
            public string server_id { get; set;}

            public CharacterItem(string name, string server_id) {
                /// Constructor
                this.name = name;
                this.server_id = server_id;
            }

            public override string ToString() {
                /// ToString representation
                return name;
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e) {
            /// Get the list of characters for the given email, the fill out the character list
            
            // Clear the character list
            lstCharacters.Items.Clear();

            // Get the email to query
            var query = txtEmail.Text.Trim();

            // Develop the HTTP request
            var request = new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:8000/account/characters"),
                Content = new StringContent(
                    JsonSerializer.Serialize(new CharacterCredentials { email = query }), 
                    Encoding.UTF8, 
                    "application/json" ),
            };

            // Send the request, get the response, and parse it
            using HttpResponseMessage response = await client.SendAsync(request);
            var respStr = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(respStr);
            JsonNode? respJson = JsonNode.Parse(respStr);

            // For each character retrieved, add them to the character list
            foreach (var item in respJson.AsArray()) {
                lstCharacters.Items.Add(new CharacterItem(item["name"].ToString(), item["server_id"].ToString()));
            }
        }

    }
}
