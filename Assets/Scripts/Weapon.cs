using UnityEngine;
using System.Collections;

public class DelegateExample : MonoBehaviour
{
	// Create a delegate definition named FloatOperationDelegate
	// This defines the parameter and return types for target functions
	public delegate float FloatOperationDelegate(float f0, float f1);
	// FloatAdd must have the same parameter and return types as

	// FloatOperationDelegate
	public float FloatAdd(float f0, float f1)
	{
		float result = f0 + f1;
		print("The sum of " + f0 + " & " + f1 + " is " + result + ".");
		return (result);
	}
	// FloatMultiply must have the same parameter and return types as well
	public float FloatMultiply(float f0, float f1)
	{
		float result = f0 * f1;
		print("The product of " + f0 + " & " + f1 + " is " + result + ".");
		return (result);
	}
	// Declare a field "fod" of the type FloatOperationDelegate
	public FloatOperationDelegate fod; // A delegate field
	void Awake()
	{
		// Assign the method FloatAdd() to fod
		fod = FloatAdd;
		// Add the method FloatMultiply(), now BOTH are called by fod
		fod += FloatMultiply;
		// Check to see whether fod is null before calling
		if (fod != null)
		{
			// Call fod(3,4); it calls FloatAdd(3,4) & then FloatMultiply(3,4)
			float result = fod(3, 4);
			// Prints: The sum of 3 & 4 is 7.
			// then Prints: The product of 3 & 4 is 12.
			print(result);
			// Prints: 12
			// Thie result is 12 because the last target method to be called
			// is the one that returns a value via the delegate.
		}
	}
}

	public enum WeaponType
{
	none, // The default / no weapon
	blaster, // A simple blaster
	spread, // Two shots simultaneously
	phaser, // Shots that move in waves
	missile, // Homing missiles
	laser, // Damage over time
	shield // Raise shieldLevel
}

// The WeaponDefinition class allows you to set the properties
// of a specific weapon in the Inspector. Main has an array
// of WeaponDefinitions that makes this possible.
// [System.Serializable] tells Unity to try to view WeaponDefinition
// in the Inspector pane. It doesn't work for everything, but it
// will work for simple classes like this!
[System.Serializable]
public class WeaponDefinition
{
	public WeaponType type = WeaponType.none;
	public string letter; // The letter to show on the power-up
	public Color color = Color.white; // Color of Collar & power-up
	public GameObject projectilePrefab; // Prefab for projectiles
	public Color projectileColor = Color.white;
	public float damageOnHit = 0; // Amount of damage caused
	public float continuousDamage = 0; // Damage per second (Laser)
	public float delayBetweenShots = 0;
	public float velocity = 20; // Speed of projectiles
}
// Note: Weapon prefabs, colors, and so on. are set in the class Main.
public class Weapon : MonoBehaviour
{
	static public Transform PROJECTILE_ANCHOR;
	public bool ____________________;
	[SerializeField]
	private WeaponType _type = WeaponType.none;
	public WeaponDefinition def;
	public GameObject collar;
	public float lastShot; // Time last shot was fired

	void Awake()
	{
		collar = transform.Find("Collar").gameObject;
	}

	void Start()
	{
		// Call SetType() properly for the default _type
		SetType(_type);
		if (PROJECTILE_ANCHOR == null)
		{
			GameObject go = new GameObject("_Projectile_Anchor");
			PROJECTILE_ANCHOR = go.transform;
		}
		// Find the fireDelegate of the parent
		GameObject parentGO = transform.parent.gameObject;
		if (parentGO.tag == "Hero")
		{
			Hero.S.fireDelegate += Fire;
		}
	}

	public WeaponType type
	{
		get { return (_type); }
		set { SetType(value); }
	}

	public void SetType(WeaponType wt)
	{
		_type = wt;
		if (type == WeaponType.none)
		{
			this.gameObject.SetActive(false);
		return;
		}
		else
		{
			this.gameObject.SetActive(true);
		}
		def = Main.GetWeaponDefinition(_type);
		collar.GetComponent<Renderer>().material.color = def.color;
		lastShot = 0; // You can always fire immediately after _type is set.
	}

	public void Fire()
	{
		// If this.gameObject is inactive, return
		if (!gameObject.activeInHierarchy) return;
		// If it hasn't been enough time between shots, return
		if (Time.time - lastShot < def.delayBetweenShots)
		{
			return;
		}
		Projectile p;
		switch (type)
		{
			case WeaponType.blaster:
				p = MakeProjectile();
				p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
				break;
			case WeaponType.spread:
				p = MakeProjectile();
				p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
				p = MakeProjectile();
				p.GetComponent<Rigidbody>().velocity = new Vector3(-.2f, 0.9f, 0) * def.velocity;
				p = MakeProjectile();
				p.GetComponent<Rigidbody>().velocity = new Vector3(.2f, 0.9f, 0) * def.velocity;
				break;
		}
	}

	public Projectile MakeProjectile()
	{
		GameObject go = Instantiate(def.projectilePrefab) as GameObject;
		if (transform.parent.gameObject.tag == "Hero")
		{
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer("ProjectileHero");
		}
		else
		{
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer("ProjectileEnemy");
		}
		go.transform.position = collar.transform.position;
		go.transform.parent = PROJECTILE_ANCHOR;
		Projectile p = go.GetComponent<Projectile>();
		p.type = type;
		lastShot = Time.time;
		return (p);
	}
}
