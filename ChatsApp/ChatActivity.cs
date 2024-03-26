using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Java.Lang;
using System.Linq;
using static ChatsApp.JsonHelper;
using static Java.IO.ObjectInputStream;
using static Android.Icu.Text.Transliterator;
using static AndroidX.RecyclerView.Widget.RecyclerView;

namespace ChatsApp
{
    [Activity(Label = "ChatActivity")]
    public class ChatActivity : Activity
    {
        Guid id = Guid.Empty;
        string chatName = "";
        ListView messageList;
        ImageButton btn;
        EditText textField;
        ArrayAdapter adapter;
        TextView chatNameView;
        ImageButton btnBack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_chat);
            messageList = FindViewById<ListView>(Resource.Id.message_list);
            Bundle extras = Intent.Extras;
            id = Guid.Parse(extras.GetString("id"));
            chatName = extras.GetString("chat_name");

            btn = FindViewById<ImageButton>(Resource.Id.send_button);
            textField = FindViewById<EditText>(Resource.Id.message_input);

            btn.Click += async(o, e) =>
            {
                string text = textField.Text;
                textField.Text = null;
                DateTime currentDate = DateTime.Today;
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                JsonHelper.Message msg = new JsonHelper.Message(id, chatName, "User", text,
                    currentDate.ToString(), currentTime.ToString());
                string response_text = GetResponse();
                currentTime = DateTime.Now.TimeOfDay;
                currentDate = DateTime.Today;
                JsonHelper.Message msg_response = new JsonHelper.Message(id, chatName, "Android", response_text,
                currentDate.ToString(), currentTime.ToString());
                await SaveMessages(msg, msg_response);
                JsonHelper.MessageJava msgJava = new JsonHelper.MessageJava(msg);
                JsonHelper.MessageJava msgResponseJava = new JsonHelper.MessageJava(msg_response);
                adapter.Add(msgJava);
                adapter.Add(msgResponseJava);
            };

            btnBack = FindViewById<ImageButton>(Resource.Id.btn_back);
            chatNameView = FindViewById<TextView>(Resource.Id.chat_title);
            chatNameView.Text = chatName;
            btnBack.Click += (o, e) =>
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                StartActivity(intent);
            };
        }
        private string GetResponse()
        {
            string response = "";
            Random random = new Random();
            int num = random.Next(0, 3);
            switch (num)
            {   
                case 0:
                    response = "Да";
                    break;
                case 1:
                    response = "Нет";
                    break;
                default:
                    response = "Возможно";
                    break;
            }
            return response;
        }
        protected override async void OnResume()
        {
            base.OnResume();

            await UpdateMessageList();
        }
        private async Task UpdateMessageList()
        {
            if (id != Guid.Empty)
            {
                List<MessageJava> messages = await ReadMessagesById(id);

                adapter = new MessageAdapter(this, this, Resource.Layout.message, messages);
                messageList.Adapter = adapter;
            }
        }



        class MessageAdapter : ArrayAdapter
        {
            private Activity activity;
            public MessageAdapter(Activity activity, Context context, int resource,
            List<MessageJava> objects)
            : base(context, resource, objects)
            {
                this.activity = activity;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var view = (convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.message,
                    parent, false)) as LinearLayout;
                TextView userName = view.FindViewById<TextView>(Resource.Id.user_name);
                TextView messageText = view.FindViewById<TextView>(Resource.Id.message_text);
                TextView messageDate = view.FindViewById<TextView>(Resource.Id.message_date);
                ImageView iconUser = view.FindViewById<ImageView>(Resource.Id.avatar_image);
                MessageJava item = (MessageJava)this.GetItem(position);
                userName.Text = item.SenderName;
                messageText.Text = item.Text;
                messageDate.Text = item.Date.Split(" ")[0] + " " + item.Time.Split(".")[0];
                if(item.SenderName == "Android")
                {
                    iconUser.SetImageResource(Resource.Drawable.android);
                }
                else
                {
                    iconUser.SetImageResource(Resource.Drawable.account_circle);
                }
                return view;
            }
        }
    }
}