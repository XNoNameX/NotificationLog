using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace NotificationLog
{
	public class MainTabWindow_OldNotifications : MainTabWindow
	{
		private float contentHeight;

		private Vector2 scrollPosition;

		private const float listItemSize = Letter.DrawHeight * 1.5f;

		private List<Letter> recordedLetters = Logger.nLogLetters;

		public override Vector2 RequestedTabSize
		{
			get
			{
				return new Vector2(600f, (float)UI.screenHeight * 0.75f);
			}
		}

		public override void PreOpen()
		{
			//Refresh the scrollable list's total height for population
			this.contentHeight = 0f;

			foreach (var let in recordedLetters)
			{
				this.contentHeight += listItemSize;
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			base.DoWindowContents(inRect);

			//Outer Rect needs to be adjusted to account for MainButtons
			Rect outRect = new Rect(inRect.x, inRect.y + MainButtonDef.ButtonHeight, inRect.width, inRect.height);

			//Inner Rect used to populate the scrollable list later
			Rect viewRect = new Rect(outRect.x + Letter.DrawWidth / 2f, outRect.y, outRect.width - Letter.DrawWidth, this.contentHeight);
			float curY = this.contentHeight;

			Widgets.BeginScrollView(inRect, ref this.scrollPosition, viewRect, true);

			for (int i = 0; i < recordedLetters.Count; i++)
			{
				var let = recordedLetters[i];

				DrawLetter(viewRect, let, curY);

				curY -= listItemSize;
			}

			Widgets.EndScrollView();
		}

		private void DrawLetter(Rect originalRect, Letter curLetter, float topY)
		{
			//Draw letter box
			Rect letRect = new Rect(originalRect.x, topY, Letter.DrawWidth, Letter.DrawHeight);

			//Draw letter icon on letter box
			GUI.color = curLetter.def.color;
			GUI.DrawTexture(letRect, curLetter.def.Icon);

			//Draw letter info
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			string text = curLetter.label;
			Rect infoRect = new Rect(letRect.x + Letter.DrawWidth * 1.5f, topY, originalRect.width - Letter.DrawWidth * 1.5f, letRect.height);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(infoRect, text);
			if (curLetter == recordedLetters.First()) //Special label for oldest notification
			{
				Text.Anchor = TextAnchor.MiddleRight;
				Widgets.Label(infoRect, "NLog_Letter_Oldest".Translate());
			}
			if (curLetter == recordedLetters.Last() && recordedLetters.Count >= 2) //Special label for latest notification
			{
				Text.Anchor = TextAnchor.MiddleRight;
				Widgets.Label(infoRect, "NLog_Letter_MostRecent".Translate());
			}
			Text.Anchor = TextAnchor.UpperLeft; //Reset TextAnchor as per RimWorld standard

			//Highlight and button
			Rect buttonRect = new Rect(letRect.x, letRect.y, originalRect.width, letRect.height);
			Widgets.DrawHighlightIfMouseover(buttonRect);
			var curChoiceLetter = curLetter as ChoiceLetter;
			if (curChoiceLetter != null) //Tooltip with some of the notification text for quality of life
			{
				string tooltipText = curChoiceLetter.text;
				if (tooltipText.Length > 100)
				{
					tooltipText = tooltipText.TrimmedToLength(100) + " ...";
				}
				TooltipHandler.TipRegion(buttonRect, tooltipText);
			}

			if (Widgets.ButtonInvisible(buttonRect, false))
			{
				curLetter.OpenLetter();
			}
		}
	}
}
