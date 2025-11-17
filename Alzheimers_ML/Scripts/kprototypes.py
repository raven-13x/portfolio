import pandas as pd
from kmodes.kprototypes import KPrototypes

pd.set_option('display.max_rows', None)
pd.set_option('display.max_columns', None)
pd.set_option('display.max_colwidth', None)

dataset_df = pd.read_csv('Data/cleaned_data.csv', encoding='utf-8')

cat_indexes = [0, 1, 2, 3, 5, 6, 7]

# Through testing, 3 clusters provided the most meaningful insight
kp_model = KPrototypes(n_clusters=3, init='Cao', random_state=10)
clusters = kp_model.fit_predict(dataset_df, categorical=cat_indexes)
dataset_df['Cluster'] = clusters
dataset_df.to_csv('Data/clustered_data.csv', index=False)

# High level summary of results
# Gender and Race/Ethnicity are dominated by 'Unknown', so caution is advised in interpreting
print(dataset_df.groupby('Cluster')['Data_Value'].describe())
print(dataset_df.groupby('Cluster')['Age_Group'].value_counts())
print(dataset_df.groupby('Cluster')['Year'].value_counts())
print(dataset_df.groupby('Cluster')['State'].value_counts())