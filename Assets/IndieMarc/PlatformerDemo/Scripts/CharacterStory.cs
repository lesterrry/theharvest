#nullable enable

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IndieMarc.Platformer {
    [RequireComponent(typeof(CharacterLogic))]
    public class CharacterStory : MonoBehaviour
    {
        public string storyId;
        public string currentEventId;
        public StoryEvent[] events;
        private CharacterLogic logic;

        private IEnumerator RunStory(string? forceEvent = null) {
            foreach (StoryEvent e in events) {
                if (forceEvent != null && e.id != forceEvent) continue;

                currentEventId = e.id;
                switch (e.type) {
                    case StoryEvent.Type.Delay: {
                        yield return new WaitForSeconds(e.delayTime);
                        break;
                    }
                    case StoryEvent.Type.Say: {
                        // while (logic.isSpeaking) yield return null;

                        logic.CallBubble(e.speech);
                        break;
                    }
                    case StoryEvent.Type.Await: {
                        goto end;
                    }
                }
            }
            end:;
        }

        void Start() {
            logic = gameObject.GetComponent<CharacterLogic>();

            string? forceEvent = null;

            if (GameProgress.stories.ContainsKey(storyId)) {
                forceEvent = GameProgress.stories[storyId];
            }

            StartCoroutine(RunStory(forceEvent));
        }
    }
}
