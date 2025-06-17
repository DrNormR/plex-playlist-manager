using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace plex_playlist_manager_4
{
    public partial class Form1 : Form
    {
        private readonly string plexServerUrl = "http://192.168.0.158:32400";
        private readonly string plexToken = "yRHcdQysrD1MmkDwoAcw";
        private HttpClient client;

        public Form1()
        {
            InitializeComponent();
        }

        // wired in Form1.Designer.cs
        private async void Form1_Load(object sender, EventArgs e)
        {
            client = new HttpClient { BaseAddress = new Uri(plexServerUrl) };
            client.DefaultRequestHeaders.Add("X-Plex-Token", plexToken);

            var playlists = await FetchPlaylists();
            listBoxPlaylists.Items.AddRange(playlists.ToArray());

            // Optional: clear DataGridView on load
            dataGridViewItems.DataSource = null;
        }

        private async Task<List<PlexPlaylist>> FetchPlaylists()
        {
            var xml = await client.GetStringAsync($"/playlists?X-Plex-Token={plexToken}");
            var doc = XDocument.Parse(xml);

            return doc.Descendants("Playlist")
                      .Select(x => new PlexPlaylist
                      {
                          Title = (string)x.Attribute("title"),
                          RatingKey = (string)x.Attribute("ratingKey")
                      })
                      .ToList();
        }

        private async Task<List<PlexItem>> FetchPlaylistItems(string ratingKey)
        {
            var xml = await client.GetStringAsync(
                $"/playlists/{ratingKey}/items?X-Plex-Token={plexToken}");
            var doc = XDocument.Parse(xml);

            return doc.Descendants("Video")
                      .Select(x =>
                      {
                          var type = (string)x.Attribute("type");
                          var title = (string)x.Attribute("title");
                          var key = (string)x.Attribute("ratingKey");
                          var pid = (string)x.Attribute("playlistItemID");
                          var year = (string)x.Attribute("year");
                          var library = (string)x.Attribute("librarySectionTitle");

                          if (type == "episode")
                          {
                              var show = (string)x.Attribute("grandparentTitle");
                              var season = int.Parse((string)x.Attribute("parentIndex"));
                              var ep = int.Parse((string)x.Attribute("index"));
                              title = $"{show} S{season:D2}E{ep:D2} – {title}";
                          }

                          return new PlexItem
                          {
                              Title = title,
                              RatingKey = key,
                              PlaylistItemID = pid,
                              Year = year,
                              Library = library
                          };
                      })
                      .ToList();
        }

        private async Task MoveOnServerAsync(PlexItem item, string afterPlaylistItemId = null)
        {
            var sel = (PlexPlaylist)listBoxPlaylists.SelectedItem;
            if (sel == null) return;

            var url = $"/playlists/{sel.RatingKey}/items/{item.PlaylistItemID}/move?X-Plex-Token={plexToken}";
            if (!string.IsNullOrEmpty(afterPlaylistItemId))
                url += $"&after={afterPlaylistItemId}";

            var resp = await client.PutAsync(url, null);
            if (!resp.IsSuccessStatusCode)
                MessageBox.Show($"Error moving “{item.Title}”: {resp.StatusCode}");
        }

        private async void btnSelectPlaylist_Click_1(object sender, EventArgs e)
        {
            if (listBoxPlaylists.SelectedItem == null) return;

            var sel = (PlexPlaylist)listBoxPlaylists.SelectedItem;
            var items = await FetchPlaylistItems(sel.RatingKey);

            dataGridViewItems.DataSource = items;
        }

        // Helper to get selected PlexItem from DataGridView
        private PlexItem GetSelectedItem()
        {
            if (dataGridViewItems.CurrentRow?.DataBoundItem is PlexItem item)
                return item;
            return null;
        }

        private int GetSelectedIndex()
        {
            return dataGridViewItems.CurrentRow?.Index ?? -1;
        }

        private void SetSelectedIndex(int idx)
        {
            if (idx >= 0 && idx < dataGridViewItems.Rows.Count)
            {
                dataGridViewItems.ClearSelection();
                dataGridViewItems.Rows[idx].Selected = true;
                dataGridViewItems.CurrentCell = dataGridViewItems.Rows[idx].Cells[0];
            }
        }

        private void RefreshDataGrid(List<PlexItem> items, int selectIdx = -1)
        {
            dataGridViewItems.AutoGenerateColumns = true; // Add this line
            dataGridViewItems.Columns.Clear();            // Clear any designer columns
            dataGridViewItems.DataSource = null;
            dataGridViewItems.DataSource = items;
            if (selectIdx >= 0 && selectIdx < items.Count)
                dataGridViewItems.Rows[selectIdx].Selected = true;

            dataGridViewItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void btnMoveUp_Click_1(object sender, EventArgs e)
        {
            int idx = GetSelectedIndex();
            if (idx <= 0) return;

            var items = (List<PlexItem>)dataGridViewItems.DataSource;
            var moving = items[idx];
            var afterId = idx > 1 ? items[idx - 2].PlaylistItemID : null;

            await MoveOnServerAsync(moving, afterId);

            items.RemoveAt(idx);
            items.Insert(idx - 1, moving);
            RefreshDataGrid(items);
            SetSelectedIndex(idx - 1);
        }

        private async void btnMoveDown_Click_1(object sender, EventArgs e)
        {
            int idx = GetSelectedIndex();
            var items = (List<PlexItem>)dataGridViewItems.DataSource;
            if (idx < 0 || idx >= items.Count - 1) return;

            var moving = items[idx];
            var afterId = items[idx + 1].PlaylistItemID;

            await MoveOnServerAsync(moving, afterId);

            items.RemoveAt(idx);
            items.Insert(idx + 1, moving);
            RefreshDataGrid(items);
            SetSelectedIndex(idx + 1);
        }

        private async void btnToTop_Click_1(object sender, EventArgs e)
        {
            var items = (List<PlexItem>)dataGridViewItems.DataSource;
            var moving = GetSelectedItem();
            if (moving == null) return;

            await MoveOnServerAsync(moving, null);

            items.Remove(moving);
            items.Insert(0, moving);
            RefreshDataGrid(items, 0);
        }

        private async void btnToBottom_Click_1(object sender, EventArgs e)
        {
            var items = (List<PlexItem>)dataGridViewItems.DataSource;
            var moving = GetSelectedItem();
            if (moving == null) return;

            var last = items.Last();
            await MoveOnServerAsync(moving, last.PlaylistItemID);

            items.Remove(moving);
            items.Add(moving);
            RefreshDataGrid(items, items.Count - 1);
        }

        private async void btnDelete_Click_1(object sender, EventArgs e)
        {
            var items = (List<PlexItem>)dataGridViewItems.DataSource;
            var moving = GetSelectedItem();
            if (moving == null) return;

            var sel = (PlexPlaylist)listBoxPlaylists.SelectedItem;
            var url = $"/playlists/{sel.RatingKey}/items/{moving.PlaylistItemID}?X-Plex-Token={plexToken}";
            var resp = await client.DeleteAsync(url);

            if (!resp.IsSuccessStatusCode)
                MessageBox.Show($"Error removing “{moving.Title}”: {resp.StatusCode}");
            else
            {
                items.Remove(moving);
                RefreshDataGrid(items);
            }
        }

        // Optional: keep selection in sync for keyboard navigation
        private void dataGridViewItems_SelectionChanged(object sender, EventArgs e)
        {
            // Optionally handle selection logic here if needed
        }

        private void dataGridViewItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optionally handle double-click logic here if needed
        }
    }

    public class PlexPlaylist
    {
        public string Title { get; set; }
        public string RatingKey { get; set; }
        public override string ToString() => Title;
    }

    public class PlexItem
    {
        public string Title { get; set; }
        public string RatingKey { get; set; }
        public string PlaylistItemID { get; set; }
        public string Year { get; set; }
        public string Library { get; set; }
        public override string ToString() => Title;
    }
}
