SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS(SELECT *
				FROM master.dbo.sysdatabases 
			   WHERE name='Test')
BEGIN 
	CREATE DATABASE Test;
END

USE Test;

IF OBJECT_ID('class') IS NULL
BEGIN
	CREATE TABLE class
	(	class_id INT NOT NULL,
		class_name NVARCHAR(20) NOT NULL,
		CONSTRAINT pk_class PRIMARY KEY(class_id)	)
END

IF OBJECT_ID('student') IS NULL
BEGIN 
	CREATE TABLE student
	(	stud_id	INT IDENTITY(1,1) NOT NULL,
		stud_name NVARCHAR(20) NOT NULL,
		class_id INT NOT NULL,
		CONSTRAINT pk_student PRIMARY KEY(stud_id),	 
		CONSTRAINT fk_student_class_id FOREIGN KEY(class_id) REFERENCES class(class_id)	)
END

IF OBJECT_ID('student_answer') IS NULL
BEGIN
	CREATE TABLE student_answer
	(	stud_id INT NOT NULL,
		stud_email VARCHAR(200) NOT NULL,
		CONSTRAINT pk_student_answer PRIMARY KEY(stud_id)	)
END

IF OBJECT_ID('studnet_choice') IS NULL
BEGIN
	CREATE TABLE student_choice
	(	stud_id INT NOT NULL,
		choice_num INT NOT NULL,
		choice_answer INT NOT NULL,
		CONSTRAINT pk_student_choice PRIMARY KEY(stud_id,choice_num),
		CONSTRAINT fk_student_choice_stud_id FOREIGN KEY(stud_id) REFERENCES student(stud_id)  )
END
