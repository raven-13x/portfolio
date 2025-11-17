import pandas as pd

def main():
    convert_df = pd.read_excel('Data/Alzheimers_Disease_dataset.xlsx', dtype=str)
    convert_df.to_csv('Data/raw_data.csv', index=False, encoding='utf-8')

    cleaned_df = pd.read_csv('Data/raw_data.csv', encoding='utf-8')

    # Filter for topics closely related to Alzheimer's
    valid_topics = ['Functional difficulties associated with subjective cognitive decline',
                    
                    'Need assistance with day-to-day activities because of subjective cognitive '
                    'decline',

                    'Subjective cognitive decline or memory loss among older adults',

                    'Talked with health care professional about subjective cognitive decline or '
                    'memory loss']
    cleaned_df = cleaned_df[cleaned_df['Topic'].isin(valid_topics)]

    # Converting numeric columns to float prior to any calculations
    # We can't use rows without data values or high/low limits
    # We also need to eliminate any 0 data values for the next section
    numeric_cols = ['Data_Value', 'Low_Confidence_Limit', 'High_Confidence_Limit']
    cleaned_df = cleaned_df.dropna(subset=numeric_cols)
    cleaned_df = cleaned_df[cleaned_df['Data_Value'] != '0']
    cleaned_df[numeric_cols] = cleaned_df[numeric_cols].astype(float)

    # Filter out records with RSE > 30% (per original report) to improve data reliability
    # With High/Low provided, a full-width confidence interval will be calculated
    # The z-value for the full-width of a 95% confidence interval is 3.92
    cleaned_df['Std_Err'] = (cleaned_df['High_Confidence_Limit'] - cleaned_df['Low_Confidence_Limit']) / 3.92
    cleaned_df['RSE'] = (cleaned_df['Std_Err'] / cleaned_df['Data_Value']) * 100
    cleaned_df = cleaned_df[cleaned_df['RSE'] <= 30.00]

    # Separate stratification 2 into gender and nationality columns for proper clustering
    # K-Prototypes can't handle missing values, so we're filling NaNs with 'Unknown'
    is_gender = cleaned_df['StratificationCategory2'] == 'Gender'
    cleaned_df.loc[is_gender, 'Gender'] = cleaned_df.loc[is_gender, 'Stratification2']
    cleaned_df['Gender'] = cleaned_df['Gender'].fillna('Unknown')

    is_race = cleaned_df['StratificationCategory2'] == 'Race/Ethnicity'
    cleaned_df.loc[is_race, 'Race/Ethnicity'] = cleaned_df.loc[is_race, 'Stratification2']
    cleaned_df['Race/Ethnicity'] = cleaned_df['Race/Ethnicity'].fillna('Unknown')

    # RowId is summarizing existing data
    # YearEnd is the same as year start
    # LocationAbbr isn't needed here when we have LocationDesc
    # Datasource contains no variance in the entire dataset
    # DataValueTypeID isn't needed here when we have the description
    # Once cleaned, Data_Value_Alt is a duplicate of Data_Value
    # Data_Value_Footnote_Symbol is essentially an ID for the actual note
    # Data_Value_Footnote is empty after cleaning is complete
    # All ID columns from ClassID on aren't needed
    # High Confidence Limit, Low Confidence Limit, Std_Err, and RSE are no longer needed
    # Data_Value_Type is redundant
    # StratificationCategory1 is all "Age Group", so we'll rename the value column
    # Geolocation is a potential PII risk that could provide the ability to link survey participants
    # StratificationCategory2/Stratification2 isn't needed once the values are meaningfully separated
    # Class is all "cognitive decline" after cleaning
    # Data_Value_Unit is all % after cleaning
    cleaned_df.drop(['RowId', 'YearEnd', 'LocationAbbr', 'Datasource', 'DataValueTypeID', 
                     'Data_Value_Alt', 'Data_Value_Footnote_Symbol', 'Data_Value_Footnote',
                     'ClassID', 'TopicID', 'QuestionID', 'LocationID', 'StratificationCategoryID1', 
                     'StratificationID1', 'StratificationCategoryID2', 'StratificationID2', 
                     'Low_Confidence_Limit', 'High_Confidence_Limit', 'Std_Err', 'RSE',
                     'Data_Value_Type', 'StratificationCategory1', 'Geolocation', 
                     'StratificationCategory2', 'Stratification2', 'Class', 'Data_Value_Unit'], 
                     axis=1, inplace=True)
    
    cleaned_df.rename(columns={'YearStart': 'Year', 
                               'LocationDesc': 'State',
                               'Stratification1': 'Age_Group'}, inplace=True)
    
    # Regional values don't necessarily encompass all states in that region
    invalid_states = ['Northeast', 'South', 'Midwest', 'United States, DC & Territories', 'West']
    cleaned_df = cleaned_df[~(cleaned_df['State'].isin(invalid_states))]

    # Normalization isn't needed since we only have one numeric column
    cleaned_df.to_csv('Data/cleaned_data.csv', index=False)

if __name__ == "__main__":
    main()