using System.Collections.Generic;
using System.Linq;
using Verse;

namespace NotificationLog
{
	public class Logger : GameComponent
	{
		internal static List<Letter> nLogLetters = new List<Letter>();

		public Logger()
		{
		}

		public Logger(Game game)
		{
		}

		public override void GameComponentTick()
		{
			if (Find.TickManager.TicksGame % GenTicks.TicksPerRealSecond == 0)
			{
				var letterStack = Find.LetterStack.LettersListForReading;

				if (letterStack.Count > 0)
				{
					var letterDifference = letterStack.Except(nLogLetters);

					foreach (var letter in letterDifference)
					{
						if (!nLogLetters.Contains(letter))
						{
							nLogLetters.Add(letter);
						}
					}
				}
			}
		}

		public override void ExposeData()
		{
			Scribe_Collections.Look(ref nLogLetters, "nlogLetters", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (nLogLetters.RemoveAll((Letter x) => x == null) != 0)
				{
					Log.Error("Notification Log :: Some letters were null.");
				}
			}
		}
	}
}
