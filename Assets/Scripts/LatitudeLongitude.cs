using UnityEngine;

public class LatitudeLongitude : MonoBehaviour
{
    public int latitude = 0;
    public int latitudeMinutes = 0;
    public int longitude = 0;
    public int longitudeMinutes = 0;

	void Awake ()
	{
        // Перемещаем transform.forward в указанные географические координаты
	    transform.rotation = Quaternion.Euler(latitude + latitudeMinutes/60, longitude + longitudeMinutes/60, 0);
	}
}
