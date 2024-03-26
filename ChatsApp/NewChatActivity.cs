using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ChatsApp.JsonHelper;
using System.Threading.Tasks;

namespace ChatsApp
{

    [Activity(Label = "NewChatActivity")]
    public class NewChatActivity : Activity
    {
        Button buttonSave;
        TextView chatName;
        ImageButton btnBack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_new_chat);
            btnBack = FindViewById<ImageButton>(Resource.Id.btn_back);
            chatName = FindViewById<TextView>(Resource.Id.chat_name);
            buttonSave = FindViewById<Button>(Resource.Id.button_save);


            buttonSave.Click += async(o, e) =>
            {
                DateTime currentDate = DateTime.Today;
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                JsonHelper.Message msg = new JsonHelper.Message(chatName.Text, "Android", "Hello!",
                    currentDate.ToString(), currentTime.ToString());
                await SaveMessage(msg);
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                StartActivity(intent);
            };

            btnBack.Click += (o, e) =>
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                StartActivity(intent);
            };
        }
       
    }
}