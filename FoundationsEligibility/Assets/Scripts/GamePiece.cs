using System.Collections;
using UnityEngine;

public class GamePiece : MonoBehaviour{

	[SerializeField] private float animTime = 0.5f;
	[SerializeField] AnimationCurve growthCurve;

	private void OnEnable()
	{
		StartCoroutine(SpawnRoutine());
	}

	IEnumerator SpawnRoutine(){
		yield return null;
		for(float t = 0 ; t <= animTime; t += Time.deltaTime){
			yield return new WaitForFixedUpdate();
			transform.localScale = Vector3.one * growthCurve.Evaluate( t/animTime);
		}
	}
}
