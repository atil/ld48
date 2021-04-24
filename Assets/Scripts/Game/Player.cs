using System.Collections;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public SpriteRenderer Visual;

        public Sprite[] MoveSprites;
        public Sprite[] IdleSprites;

        private Coroutine _idleCoroutine;

        public void PlayIdleAnim(GameDirection direction)
        {
            Vector3 scale = Visual.transform.localScale;
            if (direction == GameDirection.Down)
            {
                scale.y = -Mathf.Abs(scale.y);
            }
            else
            {
                scale.y = Mathf.Abs(scale.y);
            }

            Visual.transform.localScale = scale;


            if (_idleCoroutine != null)
            {
                CoroutineStarter.Stop(_idleCoroutine);
            }

            _idleCoroutine = CoroutineStarter.Run(PlayFxCoroutine(IdleSprites, 999, 4));
        }

        public IEnumerator PlayMoveAnim(float duration, GameDirection direction)
        {
            Vector3 scale = Visual.transform.localScale;
            if (direction == GameDirection.Down)
            {
                scale.y = -Mathf.Abs(scale.y);
            }
            else
            {
                scale.y = Mathf.Abs(scale.y);
            }

            Visual.transform.localScale = scale;
            if (_idleCoroutine != null)
            {
                CoroutineStarter.Stop(_idleCoroutine);
            }

            yield return CoroutineStarter.Run(PlayFxCoroutine(MoveSprites, duration, 9));

            PlayIdleAnim(direction);
        }

        private IEnumerator PlayFxCoroutine(Sprite[] sprites, float duration, int framesPerSecond)
        {
            float secondsPerFrame = 1f / framesPerSecond;
            int i = 0;
            float frameTimer = 0;
            for (float f = 0f; f < duration && this != null && gameObject != null; f += Time.deltaTime, frameTimer += Time.deltaTime)
            {
                if (frameTimer > secondsPerFrame)
                {
                    Visual.sprite = sprites[++i % MoveSprites.Length];
                    frameTimer = 0;
                }

                yield return null;
            }
        }
    }
}