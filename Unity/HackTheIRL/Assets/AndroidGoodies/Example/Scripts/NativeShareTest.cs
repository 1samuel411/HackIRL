#if UNITY_ANDROID
using UnityEngine;
using DeadMosquito.AndroidGoodies;
using System.Collections;

namespace AndroidGoodiesExamples
{
    public class NativeShareTest : MonoBehaviour
    {
        public Texture2D image;

        public bool withChooser = true;

        public string subject;
        public string text;

        public void OnShareClick()
        {
            AGShare.ShareTextWithImage(subject, text, image);
        }

        public void OnSendEmailClick()
        {
            var recipients = new[] {"x@gmail.com", "hello@gmail.com"};
            var ccRecipients = new[] {"cc@gmail.com"};
            var bccRecipients = new[] {"bcc@gmail.com", "bcc-guys@gmail.com"};
            AGShare.SendEmail(recipients, "subj", "body", image, withChooser, cc: ccRecipients, bcc: bccRecipients);
        }

        public void OnSendSmsClick()
        {
            AGShare.SendSms("123123123", "Hello", withChooser);
        }

        public void OnTweetClick()
        {
            AGShare.Tweet("hello! I am tweeting like a boss!", image);
        }

        public void OnShareScreenshot()
        {
            AGShare.ShareScreenshot();
        }
    }
}
#endif