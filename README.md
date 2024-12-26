# Defender for Cloud Home Assignment
This repository contains the solution for the Microsoft Defender for Cloud home assignment, 
focusing on implementing a security recommendation for AWS resources. 
The goal is to simulate how a recommendation provider can add a new assessment to improve customer security.

This project demonstrates a modular approach to implementing security recommendations for AWS resources, using the following recommendation as an example:
**[SSM.3] “Amazon EC2 instances managed by Systems Manager should have an association compliance status of COMPLIANT”**
This recommendation ensures that EC2 instances managed by AWS Systems Manager have compliant associations, preventing potential security misconfigurations.

## Answers to Assignment Questions
Below are the answers to the theoretical questions posed in the assignment:
### Question 1: What is the resource type in AWS, which this recommendation evaluates?
Resource Type in AWS:  AWS::SSM::AssociationCompliance.
### Question 2: Imagine that you need to collect data from AWS (to our product) to be able to analyze and evaluate whether a resource is healthy or not with regards to the recommendation.
#### a. Which CLI commands should be used to collect the information needed to evaluate the recommendation? What should be their input (If any)?
1.	Retrieve the list of EC2 instances:
aws ec2 describe-instances --filters "Name=tag:AmazonSSMManagedInstance,Values=true" --query "Reservations[*].Instances[*].[InstanceId]" --output text
This command filters instances by the `AmazonSSMManagedInstance` tag, which is automatically applied to instances managed by SSM.
2.	Checking Association Compliance for Each Instance:
For each instanceId retrieved in the previous command, the following command is executed:
aws ssm list-compliance-items --resource-id <instance-id> --resource-type "ManagedInstance" --filters "Key=ComplianceType,Values=Association"
Input: 
•	--resource-id: The ID of the EC2 instance being evaluated
•	--resource-type: set to ManagedInstance.
•	--filters Key=ComplianceType,Values=Association: Filter the output to include  only compliance items related to associations.
#### b. Which attributes in the CLI commands output are relevant for the evaluation?
•	From `aws ec2 describe-instances`:
InstanceIds : 
A list of all EC2 instance IDs.
•	From `aws ssm list-compliance-items`:
 An array of compliance items. Each item contains:
•	ComplianceType:
The value should be "Association" to confirm the compliance status is specific to associations.
•	Status:
Indicates whether the association is COMPLIANT or NON_COMPLIANT.
This is the primary attribute for determining compliance with the recommendation.
### Question 3: Evaluation Logic
Once the information is collected, we need to evaluate whether the resource is healthy or not. The following pseudo algorithm demonstrates how to check the compliance status of an EC2 instance:
```pseudo
function is_instance_healthy(instance_id):
// Retrieve compliance item list using list-compliance-items command
compliance_items = get_ compliance_items(instance_id) 

if  compliance_items is empty OR compliance_items.ComplianceItems is empty:
	return true     //no associations to check

// Check each compliance item for the "Association" type
for each item in compliance_items.ComplianceItems:
	if item.ComplianceType == "Association":
		//If any association is non-compliant, the instance is unhealthy
		if item.Status != "COMPLIANT": 
			return false 
// All associations are compliant, so the instance is healthy
return true

## Code Implementation
The code implements a data collector that retrieves relevant data from AWS using the AWS SDK for .NET, evaluates the compliance status according to the [SSM.3] recommendation, and identifies unhealthy EC2 instances. The implementation emphasizes modularity and extensibility, using classes such as:

*   `Resource`: Represents an AWS resource with its ID and type.
*   `ResourceValidator`: Validates resource types against the supported AWS types.
*   `Recommendation`: Represents a security recommendation and its associated health check logic.
*   `RecommendationsArsenal`: Manages a collection of recommendations and performs health checks on resources.
The `Program` class demonstrates how to use these components to check the compliance of EC2 instances with the [SSM.3] recommendation. The code also includes basic error handling using `try-catch` blocks.

The code focuses on demonstrating the logic and implementation of the recommendation check.

Adding a new recommendation is straightforward: you simply create a new `Recommendation` object and provide it with the appropriate health check function. 
This function encapsulates the specific logic for retrieving data from AWS and evaluating the health status for that particular recommendation.
The `Program.cs` file demonstrates how to add a new recommendation. The implementation of the [SSM.3] recommendation can be found there, serving as a concrete example.
