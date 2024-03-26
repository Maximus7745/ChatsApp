using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Org.Apache.Http.Authentication;
using System.Linq;
using static ChatsApp.JsonHelper;

namespace ChatsApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ListView msgList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            msgList = FindViewById<ListView>(Resource.Id.list_items);

            msgList.ItemClick += (o, e) =>
            {
                Intent intent = new Intent(ApplicationContext, typeof(ChatActivity));
                MessageJava selectedChat = (MessageJava)e.Parent.GetItemAtPosition(e.Position);
                intent.PutExtra("id", selectedChat.ChatID.ToString());
                intent.PutExtra("chat_name", selectedChat.ChatName.ToString());
                StartActivity(intent);
            };

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);


            SupportActionBar.Title = "Чаты";

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (o, e) =>
            {
                Intent intent = new Intent(ApplicationContext, typeof(NewChatActivity));
                StartActivity(intent);
            };
        }

        protected override async void OnResume()
        {
            base.OnResume();
            List<MessageJava> messages = await ReadJSON();
            ArrayAdapter adapter = new ChatAdapter(this, this, Resource.Layout.chat_item, messages);
            msgList.Adapter = adapter;
        }


        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        class ChatAdapter : ArrayAdapter
        {
            private Activity activity;
            public ChatAdapter(Activity activity, Context context, int resource,
            List<MessageJava> objects)
            : base(context, resource, objects)
            {
                this.activity = activity;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var view = (convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.chat_item, 
                    parent, false)) as LinearLayout;
                TextView chatNameView = view.FindViewById<TextView>(Resource.Id.chat_name);
                TextView chatDateView = view.FindViewById<TextView>(Resource.Id.chat_date);
                TextView messageTextView = view.FindViewById<TextView>(Resource.Id.message_text);
                MessageJava item = (MessageJava)this.GetItem(position);
                chatNameView.Text = item.ChatName;
                chatDateView.Text = item.Date.Split(" ")[0] + " " + item.Time.Split(".")[0];
                messageTextView.Text = item.SenderName + ": " + item.Text;
                return view;
            }
        }
    }
}
