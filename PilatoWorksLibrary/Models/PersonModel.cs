namespace PilatoWorksLibrary.Models;

public class PersonModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateOnly DateOfBirth { get; set; }
	public string Gender { get; set; }
	public string Number { get; set; }
	public string Email { get; set; }
	public string Address { get; set; }
	public string EmergencyName { get; set; }
	public string EmergencyRelationship { get; set; }
	public string EmergencyNumber { get; set; }
	public string ExistingMedicalCondition { get; set; }
	public string CurrentlyOnMedication { get; set; }
	public string SurgeriesTwoYears { get; set; }
	public string FitnessGoals { get; set; }
	public string CurrentFitnessLevel { get; set; }
	public string OftenExercise { get; set; }
	public string Diet { get; set; }
}
