#nullable enable

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IndieMarc.Platformer {
    [System.Serializable]
    public class StoryEvent {
        public enum Type {
            Say,
            Delay,
            Await
        };

        public string id = string.Empty;
        public Type type;
        public float delayTime = 0;
        public Speaker? speaker;
        public string speech = string.Empty;
    }

    [System.Serializable]
    public class Storyline {
        public string? id;
        public StoryEvent[] events = new StoryEvent[0];
    }

    public class StoryManager : MonoBehaviour {
        public Bubble? speechBubble;

        public string? initialStorylineId;

        public string? currentStorylineId;
        public string currentEventId = string.Empty;

        public Storyline[] lines = new Storyline[0];

        private IEnumerator RunStoryline(Storyline storyline, string? forceEvent = null) {
            currentStorylineId = storyline.id;
            foreach (StoryEvent e in storyline.events) {
                if (forceEvent != null && e.id != forceEvent) continue;

                currentEventId = e.id;
                switch (e.type) {
                    case StoryEvent.Type.Delay: {
                        yield return new WaitForSeconds(e.delayTime);
                        break;
                    }
                    case StoryEvent.Type.Say: {
                        if (speechBubble == null || e.speaker == null) break;
                        
                        speechBubble.Call(e.speech, e.speaker.anchor);
                        e.speaker.isSpeaking = true;

                        while (e.speaker.isSpeaking) yield return null;
                        
                        speechBubble.Hide();

                        break;
                    }
                    case StoryEvent.Type.Await: {
                        goto end;
                    }
                }
            }
            end:;
        }

        public void RunStoryline(string id) {
            Storyline line = lines.FirstOrDefault(line => line.id == id);

            if (line != null) StartCoroutine(RunStoryline(line));
        }

        void Start() {
            if (initialStorylineId != null) {
                Storyline initial = lines.FirstOrDefault(line => line.id == initialStorylineId);
                
                if (initial != null) StartCoroutine(RunStoryline(initial));
            }
        }
    }
}
