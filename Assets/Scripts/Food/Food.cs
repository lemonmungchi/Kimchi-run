using UnityEngine;

public enum FoodType
{
    Garlic,
    Gochu,
    Baechu,
    Golden_Baechu,
}

public class Food : MonoBehaviour
{
    [SerializeField] private FoodType foodType;

    public FoodType GetFoodType()
    {
        return foodType;
    }
}
