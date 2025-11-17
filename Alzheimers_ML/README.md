## Introduction
As a data scientist you will select a problem from the attached “Scenario List,” build a suitable AI
or ML model, and automate the entire model lifecycle from development to deployment. You will use 
the Cloud Academy Lab Environment found in the Web links section. The project documentation should 
list APIs, SDKs, and libraries used.

## Scenario
The individual scenario will be found in the “Scenario List” in the Supporting Documents section.

## Requirements
You must use the rubric to direct the creation of your submission because it provides detailed 
criteria that will be used to evaluate your work. Each requirement below may be evaluated by more 
than one rubric aspect. The rubric aspect titles may contain hyperlinks to relevant portions of the 
course.

**A. Create your own subgroup and project in GitLab using the provided "GitLab" and the "GitLab 
How-To" web links by doing the following:**
   - Clone the Project to the IDE
   - Commit with a message and push when you complete each requirement listed in parts B-G
   - Submit a copy of the GitLab repository URL in the "Comments to Evaluator" section when you 
     submit this assessment
   - Submit a copy of the repository branch history retrieved from your repository, which must 
     include commit messages and dates

Note: You may commit and push whenever you want to back up your changes, even if a requirement is 
not yet complete

Note: Wait until you have completed all the following prompts before you create your copy of the 
repository branch history

**B. Describe which problem you want to solve within the scenario from the attached "Scenario List"
including the following:**
   - Reasoning behind your choice
   - Goal in approaching the problem with artificial intelligence and machine learning

**C. Prepare your data using the dataset that corresponds to your chosen problem by doing the 
following:**
   - Normalize and clean the data
   - Categorize the data using clustering where appropriate

**D. Identify the algorithm you will use to build your artificial intelligence and machine learning
model**
   - Describe which other algorithms you considered and the pros and cons of each

**E. Discuss how you would improve the model if the size of the dataset was increased or if you had
more computing power**

**F. Discuss how the performance and efficiency of the model will be optimized**

**G. Identify three potential ethical issues in your chosen model**
   - Describe which concerns need to be mitigated in the dataset before proceeding
   - Explain the strategies for mitigating these issues in the dataset
   - Analyze the ethics of the model design
   - Explain the strategies for mitigating these issues in the model design

**H. Acknowledge sources, using APA-formatted in-text citations and references for content that is
quoted, paraphrased, or summarized**

**I. Demonstrate professional communication in the content and presentation of your submission**

## Written Responses
**B. Describe which problem you want to solve within the scenarios from the attached "Scenario 
List" including the reasoning behind your choice and the goal in approaching the problem with
artificial intelligence and machine learning**

I've chosen scenario 3, Alzheimer's Disease. Due to the complex relationships between Alzheimer's 
and patient demographics, AI is an appropriate tool here. Additionally, using the results for 
further research reduces ethical issues significantly. The COVID-19 and Hospital Staff Management 
scenarios would find well written reports or dashboards as suitable solutions. The School Attendance 
scenario can inadvertently create/introduce racial, demographic, and/or economic biases; an AI/ML 
solution could end up stigmatizing students. The Animal Incident scenario could result in Animal 
Control officers interpreting normal animal behavior as threatening in areas identified by the 
AI/ML solution.

The data provided for this scenario is a broad medical survey where each row represents different 
demographic groups. The survey contains mixed categorical and numeric data collected from 2015 to 
2022. With this data, the goal is to see if an AI can identify new demographic patterns that support 
new hypotheses for research. More specifically, this is a clustering problem in which age, race, 
gender, and location are used to discover patterns in groups with Alzheimer's related metrics.

**C. Prepare your data using the dataset that corresponds to your chosen problem**

See clean_dataset.py

**D. Identify the algorithm you will use to build your artificial intelligence and machine learning
model. Describe which other algorithms you considered and the pros and cons of each**

I've considered three algorithms: Hierarchical, DBSCAN, and K-Prototypes.
   - Hierarchical clustering doesn't require the specification of k (number of clusters); instead, 
     it produces a tree that can guide clustering. The difficulty with Hierarchical clustering is 
     that it requires Gower distances or one-hot encoding to process categorical data. This method
     also tends to struggle with data at scale.
   - DBSCAN also doesn't require k; it groups closely related data points on a distance calculation.
     However, like Hierarchical clustering, it requires either Gower distances or one-hot encoding 
     to handle categorical data.
   - K-Prototypes offers a more straightforward solution. This algorithm is designed to handle mixed 
     numeric and categorical data without need for Gower distances or one-hot encoding. 

Given that it doesn't require Gower distances or one-hot encoding, K-Prototypes is the practical 
choice for this dataset.

**E. Discuss how you would improve the model if the size of the dataset was increased or if you had
more computing power**

If the size of the dataset were increased, I wouldn't necessarily improve the model directly. 
Instead, more data could allow for different cleaning decisions, such as dropping the 'Unknowns' 
found within Gender and Race/Ethnicity. Another possibility is that the increased dataset could 
result in discovering more subtle groupings. That said, an increased dataset could produce a similar
 outcome if the additional records aren't Alzheimer's-related.

On the other hand, having more computing power would allow for exploration of additional clusters. 
It's important to note, however, that more clusters do not automatically equate to more meaningful 
data. Additional computing power could also enable the use of models that struggle with larger 
datasets, such as Hierarchical clustering. Considering these factors, K-Prototypes is still well 
suited for this dataset, and increased resources wouldn't necessarily provide much more insight here.

**F. Discuss how the performance and efficiency of the model will be optimized**

Model performance and efficiency are primarily optimized through data preparation. The original 
dataset contains over 200,000 records; by removing entries unrelated to Alzheimer's, that number is 
reduced to roughly 11,000. Additional removals are performed for missing data values and confidence 
limits. Furthermore, we drop columns that won't provide any meaningful insight. 

Outside of cleaning, optimization also comes from selecting an appropriate number of clusters; in 
doing so, we ensure a high level of efficiency while producing meaningful results. Algorithm 
selection also plays a key role, as K-Prototypes is able to handle mixed numeric and categorical 
data directly. If another algorithm was selected, Gower distances and/or one-hot encoding would be 
necessary. one-hot encoding would be especially taxing as it can involve adding numerous additional 
columns.

**G. Identify three potential ethical issues in your chosen model**

Three potential ethical issues to address are bias, privacy, and transparency/explainability:
   - Bias: Gender and race/ethnicity contain many 'unknown' values, which skews clustering.
     One potential mitigation is to remove both columns, but then visibility on those demographics 
     is lost. Another strategy is to keep both columns and to note that results should be 
     interpreted with caution. This allows us to gain some meaningful insight while highlighting the
      limitations.
   - Privacy: Personally Identifiable Information (PII) is always a concern when working with 
     medical data. Even the aggregate dataset in this scenario contains geolocation, which could be 
     used with the other data points to identify or narrow down participants. To mitigate the risk 
     of HIPAA violations, only data necessary for clustering should be retained.
   - Transparency/Explainability: K-Prototypes processes data and returns clusters, but they are not 
     immediately meaningful to humans. For insight, we have to translate the results into tables 
     and/or charts. The larger issue is that unsupervised learning is a black box. We can't know 
     with 100% accuracy now the clusters were identified. To address this, we have to remember that 
     there isn't a "right" answer in the world of unsupervised learning. The results are meant to 

     inform further exploration rather than make a groundbreaking discovery.
