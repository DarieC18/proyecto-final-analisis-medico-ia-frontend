-- Seed data for MedAnalyzerDb

-- Patients
INSERT INTO public."Patients" ("FullName", "BirthDate", "Gender", "PhoneNumber", "IdentificationNumber", "CreatedAt")
VALUES
('Juan Pérez', '1985-04-12', 'Masculino', '809-555-1234', '402-1234567-8', NOW()),
('María Gómez', '1990-10-22', 'Femenino', '809-555-5678', '001-2345678-9', NOW()),
('Luis Medina', '1978-03-05', 'Masculino', '829-555-9012', '402-9876543-2', NOW()),
('Ana Rodríguez', '1995-07-18', 'Femenino', '849-555-3456', '001-8765432-1', NOW()),
('Carlos Santana', '1982-01-30', 'Masculino', '809-555-7890', '402-6543210-5', NOW());

-- Appointments
INSERT INTO public."Appointments" ("PatientId", "DoctorId", "AppointmentDate", "Status", "Reason", "Notes", "CreatedAt")
VALUES
(1, 1, '2026-06-16 10:00:00+00', 'Pendiente', 'Dolor de cabeza severo', 'Paciente reporta cefalea intensa desde hace 3 días', NOW()),
(2, 1, '2026-06-15 14:30:00+00', 'Completada', 'Chequeo general anual', 'Todo dentro de parámetros normales', NOW()),
(3, 1, '2026-06-14 09:00:00+00', 'Cancelada', 'Lectura de análisis', 'Paciente no se presentó', NOW()),
(1, 1, '2026-06-17 11:00:00+00', 'Pendiente', 'Control de fiebre', 'Seguimiento a cuadro febril', NOW()),
(4, 1, '2026-06-17 08:30:00+00', 'Pendiente', 'Dolor lumbar crónico', 'Paciente con antecedentes de lumbalgia', NOW());

-- Symptoms
INSERT INTO public."Symptoms" ("AppointmentId", "Name", "Severity", "StartedAt", "Notes")
VALUES
(1, 'Fiebre y Escalofríos', 'Severo', '2026-06-14', 'Picos de fiebre de hasta 39.5°C por las noches'),
(1, 'Mareos y Fatiga', 'Moderado', '2026-06-15', 'Vértigo al levantarse, debilidad generalizada'),
(2, 'Cefalea tensional', 'Leve', '2026-06-14', 'Dolor de cabeza ocasional'),
(5, 'Dolor lumbar', 'Moderado', '2026-06-10', 'Dolor en zona lumbar que irradia a pierna derecha');

-- Vital Signs
INSERT INTO public."VitalSigns" ("PatientId", "AppointmentId", "Temperature", "HeartRate", "DiastolicPressure", "SystolicPressure", "OxygenSaturation", "Glucose", "MeasuredAt", "CreatedAt")
VALUES
(1, 1, 39.5, 110, 80, 120, 97.0, 95.0, '2026-06-16 10:15:00+00', NOW()),
(2, 2, 36.6, 72, 75, 115, 98.0, 88.0, '2026-06-15 14:30:00+00', NOW()),
(3, 3, 37.0, 68, 70, 110, 99.0, 90.0, '2026-06-14 09:00:00+00', NOW()),
(4, 5, 36.8, 80, 85, 130, 96.0, 102.0, '2026-06-17 08:30:00+00', NOW());

-- Medical Records
INSERT INTO public."MedicalRecords" ("PatientId", "AppointmentId", "CreatedByUserId", "DiagnosisInitial", "Notes", "CreatedAt")
VALUES
(1, 1, 0, 'Posible infección viral respiratoria', 'Paciente presenta fiebre alta, escalofríos y malestar general. Se recomienda realizar panel viral y hemograma completo.', NOW()),
(2, 2, 0, 'Paciente sano', 'Chequeo general sin hallazgos anormales. Se recomienda mantener hábitos saludables.', NOW()),
(4, 5, 0, 'Lumbalgia mecánica', 'Paciente con dolor lumbar crónico agudizado. Se recomienda fisioterapia y AINEs.', NOW());

-- AI Analyses
INSERT INTO public."AiAnalyses" ("PatientId", "AppointmentId", "DocumentId", "RequestedByUserId", "AnalysisType", "PromptUsed", "AiResponse", "ModelUsed", "CreatedAt")
VALUES
(1, 1, NULL, 0, 'Diagnóstico Asistido', 'Analizar cuadro clínico con fiebre 39.5°C, frecuencia cardíaca 110 lpm, saturación 97%, glucosa 95 mg/dL. Síntomas: fiebre y escalofríos (severo), mareos y fatiga (moderado).', 'Paciente presenta cuadro febril agudo con mareos severos. Los signos vitales sugieren una posible infección viral que requiere monitoreo. Riesgo Medio. Recomendaciones: Hidratación constante, panel viral y hemograma completo, monitoreo de curva térmica cada 4 horas.', 'GPT-4', NOW()),
(2, 2, NULL, 0, 'Chequeo General', 'Paciente sin síntomas. Signos vitales normales: temperatura 36.6°C, frecuencia cardíaca 72 lpm, presión 115/75 mmHg.', 'Paciente en estado saludable. Todos los parámetros se encuentran dentro de rangos normales. Riesgo Bajo. Se recomienda mantener chequeos anuales.', 'GPT-4', NOW());

-- Recommendations
INSERT INTO public."Recommendations" ("AiAnalysisId", "AppointmentId", "Title", "Description", "RiskLevel", "CreatedAt")
VALUES
(1, 1, 'Hidratación Intravenosa', 'Administrar hidratación constante vía intravenosa si no hay tolerancia oral', 'Alto', NOW()),
(1, 1, 'Panel Viral y Hemograma', 'Realizar panel viral completo y hemograma para identificar el agente infeccioso', 'Medio', NOW()),
(1, 1, 'Monitoreo Térmico', 'Monitorear curva térmica cada 4 horas para evaluar evolución', 'Medio', NOW()),
(2, 2, 'Mantener Chequeos Anuales', 'Continuar con chequeos médicos anuales de rutina', 'Bajo', NOW());

-- Alerts
INSERT INTO public."Alerts" ("PatientId", "AppointmentId", "Title", "Description", "Severity", "IsResolved", "CreatedAt")
VALUES
(1, 1, 'Fiebre Alta Persistente', 'Paciente presenta fiebre de 39.5°C que no cede con antipiréticos', 'Alto', false, NOW()),
(1, 1, 'Taquicardia Leve', 'Frecuencia cardíaca elevada (110 lpm) en reposo', 'Medio', false, NOW()),
(4, 5, 'Glucosa Elevada', 'Nivel de glucosa en 102 mg/dL - requiere monitoreo', 'Medio', false, NOW());
