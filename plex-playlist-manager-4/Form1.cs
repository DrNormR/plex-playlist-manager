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

                          if (type == "episode")
                          {
                              var show = (string)x.Attribute("grandparentTitle");    // ← e.g. “The Office”
                              var season = int.Parse((string)x.Attribute("parentIndex"));
                              var ep = int.Parse((string)x.Attribute("index"));
                              title = $"{show} S{season:D2}E{ep:D2} – {title}";
                          }

                          return new PlexItem
                          {
                              Title = title,
                              RatingKey = key,
                              PlaylistItemID = pid
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

        private void MoveItem(int delta)
        {
            int idx = listBoxItems.SelectedIndex;
            if (idx < 0) return;

            int newIdx = idx + delta;
            if (newIdx < 0 || newIdx >= listBoxItems.Items.Count) return;

            var itm = listBoxItems.Items[idx];
            listBoxItems.Items.RemoveAt(idx);
            listBoxItems.Items.Insert(newIdx, itm);
            listBoxItems.SelectedIndex = newIdx;
        }

        private void MoveItemTo(int newIndex)
        {
            int idx = listBoxItems.SelectedIndex;
            if (idx < 0 || idx == newIndex) return;

            var itm = listBoxItems.Items[idx];
            listBoxItems.Items.RemoveAt(idx);
            if (newIndex > idx) newIndex--;
            listBoxItems.Items.Insert(newIndex, itm);
            listBoxItems.SelectedIndex = newIndex;
        }

        // — Event handlers —
        private async void btnSelectPlaylist_Click_1(object sender, EventArgs e)
        {
            if (listBoxPlaylists.SelectedItem == null) return;

            var sel = (PlexPlaylist)listBoxPlaylists.SelectedItem;
            var items = await FetchPlaylistItems(sel.RatingKey);

            listBoxItems.Items.Clear();
            listBoxItems.Items.AddRange(items.ToArray());
        }

        private async void btnMoveUp_Click_1(object sender, EventArgs e)
        {
            int idx = listBoxItems.SelectedIndex;
            if (idx <= 0) return;

            var moving = (PlexItem)listBoxItems.Items[idx];
            var afterId = idx > 1
              ? ((PlexItem)listBoxItems.Items[idx - 2]).PlaylistItemID
              : null;

            await MoveOnServerAsync(moving, afterId);
            MoveItem(-1);
        }

        private async void btnMoveDown_Click_1(object sender, EventArgs e)
        {
            int idx = listBoxItems.SelectedIndex;
            if (idx < 0 || idx >= listBoxItems.Items.Count - 1) return;

            var moving = (PlexItem)listBoxItems.Items[idx];
            var afterId = ((PlexItem)listBoxItems.Items[idx + 1]).PlaylistItemID;

            await MoveOnServerAsync(moving, afterId);
            MoveItem(+1);
        }

        private async void btnToTop_Click_1(object sender, EventArgs e)
        {
            var moving = (PlexItem)listBoxItems.SelectedItem;
            if (moving == null) return;

            await MoveOnServerAsync(moving, null);
            MoveItemTo(0);
        }

        private async void btnToBottom_Click_1(object sender, EventArgs e)
        {
            var moving = (PlexItem)listBoxItems.SelectedItem;
            if (moving == null) return;

            var last = (PlexItem)listBoxItems.Items[listBoxItems.Items.Count - 1];
            await MoveOnServerAsync(moving, last.PlaylistItemID);
            MoveItemTo(listBoxItems.Items.Count - 1);
        }

        private async void btnDelete_Click_1(object sender, EventArgs e)
        {
            var moving = (PlexItem)listBoxItems.SelectedItem;
            if (moving == null) return;

            var sel = (PlexPlaylist)listBoxPlaylists.SelectedItem;
            var url = $"/playlists/{sel.RatingKey}/items/{moving.PlaylistItemID}?X-Plex-Token={plexToken}";
            var resp = await client.DeleteAsync(url);

            if (!resp.IsSuccessStatusCode)
                MessageBox.Show($"Error removing “{moving.Title}”: {resp.StatusCode}");
            else
                listBoxItems.Items.Remove(moving);
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
        public override string ToString() => Title;
    }
}
