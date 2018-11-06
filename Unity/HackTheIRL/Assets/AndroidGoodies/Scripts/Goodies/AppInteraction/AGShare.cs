#if UNITY_ANDROID
using UnityEngine;
using System;
using DeadMosquito.AndroidGoodies.Internal;

namespace DeadMosquito.AndroidGoodies
{
    public static class AGShare
    {
        /// <summary>
        /// Shares the text using default Android intent.
        /// </summary>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        /// <param name="withChooser">If set to <c>true</c> with chooser.</param>
        /// <param name="chooserTitle">Chooser title.</param>
        public static void ShareText(string subject, string body, bool withChooser = true,
            string chooserTitle = "Share via...")
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            var intent = new AndroidIntent(AndroidIntent.ACTION_SEND)
                .SetType(AndroidIntent.MIMETypeTextPlain);
            intent.PutExtra(AndroidIntent.EXTRA_SUBJECT, subject);
            intent.PutExtra(AndroidIntent.EXTRA_TEXT, body);

            if (withChooser)
            {
                AGUtils.StartActivityWithChooser(intent.AJO, chooserTitle);
            }
            else
            {
                AGUtils.StartActivity(intent.AJO);
            }
        }

        /// <summary>
        /// Shares the text with image using default Android intent.
        /// </summary>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        /// <param name="image">Image to send.</param>
        /// <param name="withChooser">If set to <c>true</c> with chooser.</param>
        /// <param name="chooserTitle">Chooser title.</param>
        public static void ShareTextWithImage(string subject, string body, Texture2D image, bool withChooser = true,
            string chooserTitle = "Share via...")
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (image == null)
            {
                throw new ArgumentNullException("image", "Image must not be null");
            }

            var intent = new AndroidIntent()
                .SetAction(AndroidIntent.ACTION_SEND)
                .SetType(AndroidIntent.MIMETypeImage)
                .PutExtra(AndroidIntent.EXTRA_SUBJECT, subject)
                .PutExtra(AndroidIntent.EXTRA_TEXT, body);

            var imageUri = AndroidPersistanceUtilsInternal.SaveShareImageToExternalStorage(image);
            intent.PutExtra(AndroidIntent.EXTRA_STREAM, imageUri);

            if (withChooser)
            {
                AGUtils.StartActivityWithChooser(intent.AJO, chooserTitle);
            }
            else
            {
                AGUtils.StartActivity(intent.AJO);
            }
        }

        /// <summary>
        /// Take the screenshot and share using default share intent.
        /// </summary>
        /// <param name="withChooser">If set to <c>true</c> with chooser.</param>
        /// <param name="chooserTitle">Chooser title.</param>
        public static void ShareScreenshot(bool withChooser = true, string chooserTitle = "Share via...")
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            GoodiesSceneHelper.Instance.SaveScreenshotToGallery(uri =>
            {
                var intent = new AndroidIntent()
                    .SetAction(AndroidIntent.ACTION_SEND)
                    .SetType(AndroidIntent.MIMETypeImage);

                intent.PutExtra(AndroidIntent.EXTRA_STREAM, AndroidUri.Parse(uri));

                if (withChooser)
                {
                    AGUtils.StartActivityWithChooser(intent.AJO, chooserTitle);
                }
                else
                {
                    AGUtils.StartActivity(intent.AJO);
                }
            });
        }

        /// <summary>
        /// Checks if user has any app that can handle SMS intent
        /// </summary>
        /// <returns><c>true</c>, if user has any SMS app installed, <c>false</c> otherwise.</returns>
        public static bool UserHasSmsApp()
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return false;
            }

            return CreateSmsIntent("123123123", "dummy").ResolveActivity();
        }

        private const string SmsUriFormat = "sms:{0}";

        /// <summary>
        /// Sends the sms using Android intent.
        /// </summary>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="message">Message.</param>
        /// <param name="withChooser">If set to <c>true</c> with chooser.</param>
        /// <param name="chooserTitle">Chooser title.</param>
        public static void SendSms(string phoneNumber, string message, bool withChooser = true,
            string chooserTitle = "Send SMS...")
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            var intent = CreateSmsIntent(phoneNumber, message);
            if (withChooser)
            {
                AGUtils.StartActivityWithChooser(intent.AJO, chooserTitle);
            }
            else
            {
                AGUtils.StartActivity(intent.AJO);
            }
        }

        private static AndroidIntent CreateSmsIntent(string phoneNumber, string message)
        {
            var intent = new AndroidIntent(AndroidIntent.ACTION_VIEW);

            if (AGDeviceInfo.SDK_INT >= AGDeviceInfo.VersionCodes.KITKAT)
            {
                var uri = AndroidUri.Parse(string.Format(SmsUriFormat, phoneNumber));
                intent.SetData(uri);
            }
            else
            {
                intent.SetType("vnd.android-dir/mms-sms");
                intent.PutExtra("address", phoneNumber);
            }

            intent.PutExtra("sms_body", message);
            return intent;
        }

        /// <summary>
        /// Checks if the user has any email app installed.
        /// </summary>
        /// <returns><c>true</c>, if the user has any email app installed, <c>false</c> otherwise.</returns>
        public static bool UserHasEmailApp()
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return false;
            }

            return CreateEmailIntent(new[] {"dummy@gmail.com"}, "dummy", "dummy").ResolveActivity();
        }

        /// <summary>
        /// Sends the email using Android intent.
        /// </summary>
        /// <param name="recipients">Recipient email addresses.</param>
        /// <param name="subject">Subject of email.</param>
        /// <param name="body">Body of email.</param>
        /// <param name="attachment">Image to send.</param>
        /// <param name="withChooser">If set to <c>true</c> with chooser.</param>
        /// <param name="chooserTitle">Chooser title.</param>
        /// <param name="cc">Cc recipients. Cc stands for "carbon copy."
        /// Anyone you add to the cc: field of a message receives a copy of that message when you send it.
        /// All other recipients of that message can see that person you designated as a cc
        /// </param>
        /// <param name="bcc">Bcc recipients. Bcc stands for "blind carbon copy."
        /// Anyone you add to the bcc: field of a message receives a copy of that message when you send it.
        /// But, bcc: recipients are invisible to all the other recipients of the message including other bcc: recipients.
        /// </param>
        public static void SendEmail(string[] recipients, string subject, string body,
            Texture2D attachment = null, bool withChooser = true, string chooserTitle = "Send mail...",
            string[] cc = null, string[] bcc = null)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            var intent = CreateEmailIntent(recipients, subject, body, attachment, cc, bcc);
            if (withChooser)
            {
                AGUtils.StartActivityWithChooser(intent.AJO, chooserTitle);
            }
            else
            {
                AGUtils.StartActivity(intent.AJO);
            }
        }

        private static AndroidIntent CreateEmailIntent(string[] recipients, string subject, string body,
            Texture2D attachment = null, string[] cc = null, string[] bcc = null)
        {
            var uri = AndroidUri.Parse("mailto:");
            var intent = new AndroidIntent()
                .SetAction(AndroidIntent.ACTION_SENDTO)
                .SetData(uri)
                .PutExtra(AndroidIntent.EXTRA_EMAIL, recipients)
                .PutExtra(AndroidIntent.EXTRA_SUBJECT, subject)
                .PutExtra(AndroidIntent.EXTRA_TEXT, body);
            if (cc != null)
            {
                intent.PutExtra(AndroidIntent.EXTRA_CC, cc);
            }
            if (bcc != null)
            {
                intent.PutExtra(AndroidIntent.EXTRA_BCC, bcc);
            }

            if (attachment != null)
            {
                var imageUri = AndroidPersistanceUtilsInternal.SaveShareImageToExternalStorage(attachment);
                intent.PutExtra(AndroidIntent.EXTRA_STREAM, imageUri);
            }

            return intent;
        }

        // TWITTER
        private const string TwitterPackage = "com.twitter.android";
        private const string TweetActivity = "com.twitter.android.composer.ComposerActivity";

        /// <summary>
        /// Determines if twitter is installed.
        /// </summary>
        /// <returns><c>true</c> if twitter is installed; otherwise, <c>false</c>.</returns>
        public static bool IsTwitterInstalled()
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return false;
            }

            return AGDeviceInfo.IsPackageInstalled(TwitterPackage);
        }

        /// <summary>
        /// Tweet the specified text and image. Will fallback to browser if official twitter app is not installed.
        /// </summary>
        /// <param name="tweet">Text to tweet.</param>
        /// <param name="image">Image to tweet.</param>
        public static void Tweet(string tweet, Texture2D image = null)
        {
            if (AGUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (IsTwitterInstalled())
            {
                var intent = new AndroidIntent(AndroidIntent.ACTION_SEND)
                    .SetType(AndroidIntent.MIMETypeTextPlain)
                    .PutExtra(AndroidIntent.EXTRA_TEXT, tweet)
                    .SetClassName(TwitterPackage, TweetActivity);

                var imageUri = AndroidPersistanceUtilsInternal.SaveShareImageToExternalStorage(image);
                intent.PutExtra(AndroidIntent.EXTRA_STREAM, imageUri);

                AGUtils.StartActivity(intent.AJO);
            }
            else
            {
                Application.OpenURL("https://twitter.com/intent/tweet?text=" + WWW.EscapeURL(tweet));
            }
        }
    }
}

#endif