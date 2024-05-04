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

        static HttpClient client = new HttpClient();

        public Form1() {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e) {
            lstItems.Items.Clear();
            using HttpResponseMessage response = await client.GetAsync("http://localhost:8000/item/all");
            var respStr = await response.Content.ReadAsStringAsync();
            JsonNode? respJson = JsonNode.Parse(respStr);
            foreach ( var item in respJson.AsArray() ) {
                Item next = new Item(item["id"].ToString(), item["name"].ToString());
                lstItems.Items.Add(next);
            }
        }

        private void btnAddInventory_Click(object sender, EventArgs e) {
            var items = lstItems.SelectedItems;
            foreach (var item in items) {
                lstInventory.Items.Add(item);
            }
        }

        private void btnRemInventory_Click(object sender, EventArgs e) {
            var indices = lstInventory.SelectedIndices;
            foreach (int index in indices) {
                lstInventory.Items.RemoveAt(index);
            }
        }

        public class ItemCredentials {
            public int id { get; set; }
        }

        public class Item {
            public string id { get; set; }
            public string name { get; set; }

            public Item(string id, string name) {
                this.id = id;
                this.name = name;
            }

            public async Task GetName(int id) {
                var request = new HttpRequestMessage {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://localhost:8000/item"),
                    Content = new StringContent(
                    JsonSerializer.Serialize(new ItemCredentials { id = id }),
                    Encoding.UTF8,
                    "application/json"),
                };
                using HttpResponseMessage response = await client.SendAsync(request);
                var respStr = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(respStr);
                JsonNode respJson = JsonNode.Parse(respStr);
                this.name = respJson[0]["name"].ToString();

            }

            public override string ToString() {
                return name;
            }
        }

        public class InventoryCredentials {
            public string server_id { get; set; }
            public string name { get; set; }
        }

        private async void lstCharacters_SelectedIndexChanged(object sender, EventArgs e) {
            lstInventory.Items.Clear();
            CharacterItem? selected = lstCharacters.SelectedItem as CharacterItem;
            var request = new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:8000/inventory/all"),
                Content = new StringContent(
                    JsonSerializer.Serialize(new InventoryCredentials { server_id=selected.server_id, name=selected.name }),
                    Encoding.UTF8,
                    "application/json"),
            };
            using HttpResponseMessage response = await client.SendAsync(request);
            var respStr = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(respStr);
            JsonNode? respJson = JsonNode.Parse(respStr);
            foreach (var item in respJson.AsArray()) {
                Item next = new Item("1", "temp");
                await next.GetName(int.Parse(next.id));
                lstInventory.Items.Add(next);
            }

        }

        public class InvSaveCred {
            public int server_id { get; set; }
            public string name { get; set; }
            public List<List<int>> inventory { get; set; }
        }

        private void btnSaveInv_Click(object sender, EventArgs e) {
            var query = txtEmail.Text.Trim();
            foreach b
            var request = new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:8000/inventory/all"),
                Content = new StringContent(
                    JsonSerializer.Serialize(new inv{ email = query }),
                    Encoding.UTF8,
                    "application/json"),
            };
            using HttpResponseMessage response = await client.SendAsync(request);
            var respStr = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(respStr);
            JsonNode? respJson = JsonNode.Parse(respStr);
            foreach (var item in respJson.AsArray()) {
                lstCharacters.Items.Add(new CharacterItem(item["name"].ToString(), item["server_id"].ToString()));
            }
        }

        public class CharacterCredentials {
            public string email { get; set;}
        }

        public class CharacterItem {
            public string name { get; set; }
            public string server_id { get; set;}

            public CharacterItem(string name, string server_id) {
                this.name = name;
                this.server_id = server_id;
            }

            public override string ToString() {
                return name;
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e) {
            lstCharacters.Items.Clear();
            var query = txtEmail.Text.Trim();
            var request = new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:8000/account/characters"),
                Content = new StringContent(
                    JsonSerializer.Serialize(new CharacterCredentials { email = query }), 
                    Encoding.UTF8, 
                    "application/json" ),
            };
            using HttpResponseMessage response = await client.SendAsync(request);
            var respStr = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(respStr);
            JsonNode? respJson = JsonNode.Parse(respStr);
            foreach (var item in respJson.AsArray()) {
                lstCharacters.Items.Add(new CharacterItem(item["name"].ToString(), item["server_id"].ToString()));
            }
        }

    }
}
