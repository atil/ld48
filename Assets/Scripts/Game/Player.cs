using System.Collections;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public SpriteRenderer Visual;

        public Sprite[] MoveSprites;
        public Sprite[] IdleSprites;
        public Sprite DeadSprite;

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

        public IEnumerator PlayDeadAnim(float duration, GameDirection direction)
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

            Visual.sprite = DeadSprite;
            
            yield return CoroutineStarter.Run(PlayFlipCoroutine(duration));
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

        private IEnumerator PlayFlipCoroutine(float duration)
        {
            float radiantPerFrame = 360f / (duration / Time.deltaTime);
            radiantPerFrame *= 0.5f;

            var initEuler = Visual.transform.eulerAngles;

            for (float timer = 0; timer < duration; timer += Time.deltaTime)
            {
                Visual.transform.eulerAngles -= new Vector3(0, 0, radiantPerFrame);
                yield return new WaitForEndOfFrame();
            }

            Visual.transform.eulerAngles = initEuler;
        }
    }
}