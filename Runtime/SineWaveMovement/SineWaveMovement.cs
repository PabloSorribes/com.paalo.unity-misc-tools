using UnityEngine;

namespace OutHere
{
	public class SineWaveMovement : MonoBehaviour
	{
		Vector3 startPos;
		public float amplitude = 5f;
		public float period = 0.5f;

		public enum SineModes
		{
			Normal,
			BounceUp,
			BounceUpWithPause,
			UIAlertThrob
		}
		public SineModes sineModes = SineModes.Normal;

		protected void Start()
		{
			startPos = transform.position;
		}

		protected void Update()
		{
			transform.position = GetSineWaveMovement(period, amplitude, startPos, Vector3.up);
		}

		private Vector3 GetSineWaveMovement(float period, float amplitude, Vector3 startPos, Vector3 direction)
		{
			float theta = Time.timeSinceLevelLoad / period;

			float distance = 0f;

			switch (sineModes)
			{
				case SineModes.Normal:
					distance = amplitude * Mathf.Sin(theta);    //Up-down (normal sine wave)	
					break;
				case SineModes.BounceUp:
					distance = amplitude * Mathf.Abs(Mathf.Sin(theta));     //Bounce Up (directly)
					break;
				case SineModes.BounceUpWithPause:
					distance = amplitude * Mathf.Clamp01(Mathf.Sin(theta));     //Bounce Up, but pause for 2 periods between the bounces
					break;
				case SineModes.UIAlertThrob:
					distance = amplitude * Mathf.Clamp(Mathf.Sin(theta), 0.6f, 1f);     //UI alert throb
					break;
				default:
					break;
			}

			return startPos + direction * distance;
		}
	}
}