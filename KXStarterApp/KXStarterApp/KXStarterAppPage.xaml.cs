﻿using Xamarin.Forms;
using System;
using System.Collections.Generic;
using KinveyXamarin;

namespace KXStarterApp
{
	public partial class KXStarterAppPage : ContentPage
	{
		private string app_key = "kid_byWWRXzJCe";
		private string app_secret = "4a58018febe945fea5ba76c08ce1e870";
		private string user = "testuser";
		private string pass = "testpass";

		public KXStarterAppPage()
		{
			InitializeComponent();
			try
			{
				Client c = new Client.Builder(app_key, app_secret)
									 //.setFilePath (NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].ToString ())
									 .setFilePath(DependencyService.Get<ISQLite>().GetPath())
									 .setOfflinePlatform(DependencyService.Get<ISQLite>().GetConnection())
									//.setLogger (delegate (string msg) { Console.WriteLine (msg); })
									.build();
			}
			catch (Exception e)
			{
				DisplayAlert("General Exception", e.Message, "OK");
			}
		}

		async void OnButtonClicked(object sender, EventArgs args)
		{
			try
			{
				if (!Client.SharedClient.CurrentUser.isUserLoggedIn())
				{
					await Client.SharedClient.CurrentUser.LoginAsync(user, pass);
				}

				DataStore<Book> dataStore = DataStore<Book>.GetInstance(DataStoreType.SYNC, "books");

				Button button = (Button)sender;

				if (button.Text == "Add Book")
				{
					Book b = new Book();
					b.Title = "Crime and Punishment";
					b.Genre = "Fiction";
					await dataStore.SaveAsync(b);

					await DisplayAlert("Book Added!",
										"The button labeled '" + button.Text + "' has been clicked, and the book '" + b.Title + "' has been added.",
										"OK");
				}
				else if (button.Text == "Push")
				{
					DataStoreResponse dsr = await dataStore.PushAsync();
					await DisplayAlert("Local Data Pushed!",
										"The button labeled '" + button.Text + "' has been clicked, and " + dsr.Count + " book(s) has/have been pushed to Kinvey.",
										"OK");
				}
				else if (button.Text == "Pull")
				{
					List<Book> books = await dataStore.PullAsync();
					await DisplayAlert("Local Data Pulled!",
										"The button labeled '" + button.Text + "' has been clicked, and " + books.Count + " book(s) has/have been pulled from Kinvey.",
										"OK");
				}
				else if (button.Text == "Sync")
				{
					DataStoreResponse dsr = await dataStore.SyncAsync();
					await DisplayAlert("Local Data Synced!",
										"The button labeled '" + button.Text + "' has been clicked, and " + dsr.Count + " book(s) has/have been synced with Kinvey.",
										"OK");
				}
			}
			catch (KinveyException ke)
			{
				await DisplayAlert("Kinvey Exception",
								   ke.Reason + " | " + ke.Explanation + " | " + ke.Fix,
								   "OK");
			}
			catch (Exception e)
			{
				await DisplayAlert("General Exception",
								   e.Message,
								   "OK");
			}
		}
	}

	public interface ISQLite
	{
		SQLite.Net.Interop.ISQLitePlatform GetConnection();
		string GetPath();
	}
}

