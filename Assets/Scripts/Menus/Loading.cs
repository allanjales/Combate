using UnityEngine;

public class Loading : MonoBehaviour
{
	void Update()
	{
		this.transform.eulerAngles = new Vector3(
			this.transform.eulerAngles.x,
			this.transform.eulerAngles.y + 10,
			this.transform.eulerAngles.z
		);
	}
}