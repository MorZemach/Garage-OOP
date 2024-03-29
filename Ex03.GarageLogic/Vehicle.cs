﻿namespace Ex03.GarageLogic
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public abstract class Vehicle
    {
        protected readonly string r_LicenseNumber;
        protected List<Wheel> m_Wheels;
        protected Engine m_Engine;
        protected string m_ModelName;

        public Vehicle(string i_LicenseNumber, eWheelsNumber i_WheelsNumber, float i_MaxAirPressure,
            Engine i_Engine)
        {
            r_LicenseNumber = i_LicenseNumber;
            m_Engine = i_Engine;
            m_Wheels = new List<Wheel>((int)i_WheelsNumber);

            for(int i = 0; i < (int)i_WheelsNumber; i++)
            {
                m_Wheels.Add(new Wheel(i_MaxAirPressure));
            }
        }

        public void UpdateEnergyAmount(float i_EnergyToAdd, FuelEngine.eFuelType i_FuelType = 0)
        {
            Engine.GetEnergy(i_EnergyToAdd, i_FuelType);
        }

        public string LicenseNumber { get { return r_LicenseNumber; } }

        public string ModelName 
        { 
            get { return m_ModelName; }
            set 
            { 
                if(ValidationCheck.ValidationCheckForName(value) == true)
                {
                    m_ModelName = value;
                }
            }
        }

        public Engine Engine { get { return m_Engine; } }

        public float EnergyLevel
        {
            get { return Engine.CurrentStatusOfEnergy; }
            set { Engine.CurrentStatusOfEnergy = value; }
        }

        public List<Wheel> Wheels
        {
            get { return m_Wheels; }
        }

        public Wheel this[int i_NumberOfWheel]
        {
            get { return m_Wheels[i_NumberOfWheel]; }
            set { m_Wheels[i_NumberOfWheel] = value; }
        }

        public virtual List<QuestionForVehicleInformation> AskForDataToVehicle()
        {
            List<QuestionForVehicleInformation> questionsForVehicleInfo = new List<QuestionForVehicleInformation>();

            questionsForVehicleInfo.Add(new QuestionForVehicleInformation
                ("model name", QuestionForVehicleInformation.eValidationCheckType.ValidStringCheck));
            questionsForVehicleInfo.Add(Engine.AskForDataToEngine());
            questionsForVehicleInfo.AddRange(m_Wheels[0].AskForDataToWheel());

            return questionsForVehicleInfo;
        }

        public virtual bool SetRemainingVehicleDetails(List<string> i_CurrentInfoToVehicle) 
        {
            bool vehicleDitailsSetSuccessfully = false;
            float energyLevelInput, wheelCurrentAirPressure;

            ModelName = i_CurrentInfoToVehicle[0];
            if(float.TryParse(i_CurrentInfoToVehicle[1], out energyLevelInput) == true &&
                float.TryParse(i_CurrentInfoToVehicle[3], out wheelCurrentAirPressure) == true)
            {
                EnergyLevel = energyLevelInput;
                foreach (Wheel wheel in m_Wheels)
                {
                    wheel.ManufacturerName = i_CurrentInfoToVehicle[2];
                    wheel.CurrentAirPressure = wheelCurrentAirPressure;
                }
                vehicleDitailsSetSuccessfully = true;
            }

            return vehicleDitailsSetSuccessfully;
        }
            
        public override string ToString()
        {
            StringBuilder vehicleDetails = new StringBuilder();

            vehicleDetails.Append(string.Format
                ("license number: {0}, model: {1}, energy level: {2}.{3}Wheels details:{4}",
                LicenseNumber, ModelName, EnergyLevel, Environment.NewLine, Environment.NewLine));

            foreach(Wheel wheel in Wheels)
            {
                vehicleDetails.Append(wheel.ToString());
                vehicleDetails.Append(Environment.NewLine);
            }

            vehicleDetails.Append(Engine.ToString());
            vehicleDetails.Append(Environment.NewLine);

            return vehicleDetails.ToString();
        }

        public enum eWheelsNumber
        {
            Two = 2,
            Four = 4,
            Sixteen = 16
        }

        public class Wheel
        {
            public const float k_MinAirPressure = 1;
            private readonly float r_MaxAirPressure;
            private string m_ManufacturerName;
            private float m_CurrentAirPressure;

            public Wheel(float i_MaxAirPressure)
            {
                r_MaxAirPressure = i_MaxAirPressure;
            }

            public float CurrentAirPressure
            {
                get { return m_CurrentAirPressure; }
                set
                {
                    if (r_MaxAirPressure >= value)
                    {
                        m_CurrentAirPressure = value;
                    }
                    else
                    {
                        throw new ValueOutOfRangeException(0, r_MaxAirPressure, "Air pressure");
                    }
                }
            }

            public float MaxAirPressure { get { return r_MaxAirPressure; } }

            public string ManufacturerName
            {
                get { return m_ManufacturerName; }
                set 
                { 
                    if(ValidationCheck.ValidationCheckForName(value) == true)
                    {
                        m_ManufacturerName = value;
                    }
                }
            }

            public void Inflating(float i_AddAirPressure)
            {
                CurrentAirPressure += i_AddAirPressure;
            }

            public override string ToString()
            {
                return string.Format("Manufacture is {0}, maximum air pressure is {1} and current air pressure is {2}.",
                    ManufacturerName, MaxAirPressure, CurrentAirPressure);
            }

            public List<QuestionForVehicleInformation> AskForDataToWheel()
            {
                List<QuestionForVehicleInformation> questionsForWheelsInfo =
                    new List<QuestionForVehicleInformation>();

                questionsForWheelsInfo.Add(new QuestionForVehicleInformation
                    ("wheel manufacturer name", QuestionForVehicleInformation.eValidationCheckType.ValidStringCheck));
                questionsForWheelsInfo.Add(new QuestionForVehicleInformation
                    ("wheel current air pressure", QuestionForVehicleInformation.eValidationCheckType.ValidRangeCheck,
                    (int)k_MinAirPressure, (int)r_MaxAirPressure));

                return questionsForWheelsInfo;
            }
        }

        public class QuestionForVehicleInformation
        {
            public readonly eValidationCheckType r_DataTypeForChecking;
            public readonly string r_AskForData;
            public readonly int r_MinRange;
            public readonly int r_MaxRange;

            public enum eValidationCheckType
            {
                ValidStringCheck = 1,
                ValidBooleanCheck,
                ValidRangeCheck,
            }

            public eValidationCheckType validationCheckType
            {
                get { return r_DataTypeForChecking; }
            }

            public bool AnswerValidationCheck(string i_Answer)
            {
                bool isVaildAnser =  false;
                
                if(validationCheckType == eValidationCheckType.ValidBooleanCheck)
                {
                    isVaildAnser = ValidationCheck.ValidationBooleanDataCheck(i_Answer);
                }
                else if(validationCheckType == eValidationCheckType.ValidStringCheck)
                {
                    isVaildAnser = ValidationCheck.ValidationCheckForName(i_Answer);
                }
                else if(validationCheckType == eValidationCheckType.ValidRangeCheck)
                {
                    isVaildAnser = ValidationCheck.ValidationCheckForDataRangeValue(i_Answer, this);
                }

                return isVaildAnser;
            }

            public QuestionForVehicleInformation(string i_AskForData, eValidationCheckType i_DataType)
            {
                r_AskForData = i_AskForData;
                r_DataTypeForChecking = i_DataType;
            }

            public QuestionForVehicleInformation(string i_AskForData, eValidationCheckType i_DataType,
                int i_MinRange, int i_MaxRange)
            {
                r_AskForData = i_AskForData;
                r_DataTypeForChecking = i_DataType;
                r_MinRange = i_MinRange;
                r_MaxRange = i_MaxRange;
            }
        }
    }
}

