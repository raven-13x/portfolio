## Introduction
You are a lead developer for a municipality that is currently without an automated system to assist 
with dispatching ambulances for emergency calls. You have been asked to design, build, and test two 
working prototypes to identify the best solution for: 1. selecting optimal routes to the call 
location, and 2. selecting the appropriate hospital to handle the call. Your project will consist of
 four phases comprised of written material and code. The project documentation should list APIs, 
 SDKs and libraries used.


## Scenario
In this performance assessment, you will implement two separate Python ambulance dispatch prototypes
 that differ only in the algorithms identified in part C to find the fastest route to a call 
 location. These are the requirements that should be used to develop the project as you build the 
 data structures, algorithms, and code for the ambulance dispatch project.

<ol>
    <li>
        Implement any data structures, algorithms, and functions to apply the data in the simulation
        files (See Supporting Documents section for .csv files) when dispatching ambulances to 
        emergency calls.
    </li>
    <li>
        Implement any data structures, algorithms, and functions to dispatch ambulances according to
         the following rules:
    </li>
        <ul>
            <li>
                Process the highest priority calls first and in the order in which they were 
                received for the priority currently being processed.
            </li>
            <li>
                Determine the fastest route to the call for all ambulances from their staging 
                location to the call location, apply any delay times, and select the ambulance with 
                the fastest time to the call location.
            </li>
            <li>
                Update a dispatch record containing the following attributes: Call ID, Call Type, 
                Call Location, Selected Ambulance, Route to Call Location, and Time to the Call 
                Location.
            </li>
            <li>
                Append the log file /var/log/ambulance_call_log.csv with a string of name-value 
                pairs separated by commas for the data contained in the dispatch record.
            </li>
            <li>
                After appending the dispatch record to the log file, the ambulance location is reset
                 to its staging location and available for service.
            </li>
            <li>
                Process the next call until all calls are dispatched.
            </li>
        </ul>
    <li>Implement algorithm performance metrics for each prototype.</li>
        <ul>
            <li>
                Add embedded counters to the code for measuring the total execution time finding the
                 fastest route defined in Aspect C. The code for the embedded counters can be found 
                 in the supporting document "Embedded Counters".
            </li>
            <li>
                Collect and display the total execution time spent finding the fastest route for all
                 dispatched calls.
            </li>
        </ul>
</ol>


## Requirements
You must use the rubric to direct the creation of your submission because it provides detailed 
criteria that will be used to evaluate your work. Each requirement below may be evaluated by more 
than one rubric aspect.

<ol type="A">
    <li>
        Create your subgroup and project in GitLab using the provided web link and the "GitLab 
        How-To" web link by doing the following:
    </li>
        <ul>
            <li>Clone the project to the IDE.</li>
            <li>
                Commit with a message and push when you complete each requirement listed in parts 
                E-F.
            </li>
            <li>
                Submit a copy of the GitLab repository URL in the "Comments to Evaluator" section 
                when you submit this assessment.
            </li>
            <li>
                Submit a copy of the repository branch history retreived from your repository, which
                must include the commit messages and dates
            </li>
        </ul>
    <p>
        Note: You may commit and push whenever you want to back up your changes, even if a 
        requirement is not yet complete.
    </p>
    <p>
        Note: Wait until you have completed all the following prompts before you create your 
        copy of the repository branch history.
    </p>
    <li>Describe the design of the ambulance dispatch system including the following:</li>
        <ul>
            <li>Discuss the overall operation of the application.</li>
            <li>Delineate how the application will process the simulation files.</li>
            <li>Desribe how the application will manage call priorities.</li>
        </ul>
    <li>
        Identify two different algorithms, one for each prototype, that you will use to find the
        fastest route to a call.
    </li>
        <ul>
            <li>
                Explain the operation of each algorithm that can be used to calculate the fastest 
                route.
            </li>
            <li>Describe how each algorithm will be used in the application.</li>
            <li>
                Calculate Big O time complexity and the Big O space complexity of each algorithm.
            </li>
            <li>Explain the advantages and disadvantages of each algorithm.</li>
        </ul>
    <li>
        Construct two ambulance dispatch prototypes that differ only in the algorithms defined in
        part C.
    </li>
    <li>Implement Prototype One by doing the following:</li>
        <ul>
            <li>
                Create a Python project named [StudentID]_D795_PT1 using PyCharm in the virtual
                machine.
            </li>
            <li>
                Implement each of the requirements listed in the scenario using the first algorithm 
                designed in part C.
            </li>
            <li>Add a separate commit message in GitLab when each requirement is completed.</li>
            <li>
                Provide a link in the Student Submission files to the completed Prototype One
                project in the GitLab repository.
            </li>
        </ul>
    <li>Implement Prototype Two by doing the following:</li>
        <ul>
            <li>
                Create a Python project named [StudentID]_D795_PT2 using PyCharm in the virtual
                machine.
            </li>
            <li>Clone the Prototype One project in its entirety</li>
            <li>
                Implement each of the requirements listed in the scenario using the second algorithm 
                designed in part C.
            </li>
            <li>Add a separate commit message in GitLab when each requirement is completed.</li>
            <li>
                Provide a link in the Student Submission files to the completed Prototype Two
                project in the GitLab repository.
            </li>
        </ul>
    <li>Conduct performance testing of Prototype One and Prototype Two by doing the following:</li>
        <ul>
            <li>
                Execute Prototype One 10 times and calculate and present the average execution time
                for finding the fastest route algorithm.
            </li>
            <li>
                Execute Prototype Two 10 times and calculate and present the average execution time 
                for finding the fastest route algorithm.
        </ul>
    <li>Using the metrics calculated in part G, do the following:</li>
        <ul>
            <li>Compare the performance of both algorithms</li>
            <li>
                Provide one recommendation for improving each algorithm based on your interpretation
                of the performance testing.
            </li>
        </ul>
    <li>
        Acknowledge sources, using APA-formatted in-text citations and references, for content that 
        is quoted, paraphrased, or summarized
    </li>
    <li>
        Demonstrate professional communication in the content and presentation of your submission.
    </li>
</ol>

## Written Responses
<p>B. Describe the design of the ambulance dispatch system</p>
<p>
    This Python prorgam automates emergency call dispatch. At a high level, the program is designed 
    to dispatch an ambulance to a call based on call priority and estimated response time. All data 
    is stored in csv files which the program will process using the Pandas package. Calls are 
    prioritized based on the call_priority.csv <strong>then</strong> by call order (oldest to 
    newest).
</p>
<br/>

<p>
    C. Identify two different algorithms, one for each prototype, that you will use to find the fastest
    route to a call
</p>

#### Algorithm 1
<p>
    The algorithm used in Project 1 is a simple greedy algorithm. This algorithm operates under
    the assumption that the direct route provided from staging location to call location is the 
    fastest route for each ambulance. For each call, the algorithm iterates through each ambulance
    and starts by checking if the ambulance is already on location. If the ambulance is already on
    location, all processing ceases, and that ambulance is dispatched. In every other case, the
    algorithm queries the direct route and calculates the total travel time for each ambulance. Once
    complete, the algorithm returns the ambulance with the shortest travel time and dispatches it.
</p>
<p>
    In the application, this algorithm processes each call individually as it comes in. Call order
    is established prior to the first algorithm call. Overall, this algorithm prioritizes processing
    speed over route optimization.
</p>
<p>
    Time Complexity: O(n) - The processing time of this algorithm is purely based on the number of 
    available ambulances.
</p>
<p>
    Space Complexity: O(1) - Regardless of input size, all data structures and variables remain
    (relatively) constant in size. The number and type of data doesn't change; it's the specific
    values that do.
</p>
<p>
    The primary advantage of this algorithm is its processing speed, which results in immediate
    response to emergency calls. Additionally, its linear processing makes it simple for others to
    maintain and debug as necessary.
</p>
<p>
    The primary disadvantage is the lack of route optimization. While dispatch may be fast, the
    provided route could cost vital minutes when responding to a serious emergency. This is
    especially true in a scenario where the location_network.csv is constantly updating.
</p>

#### Algorithm 2
<p>
    The algorithm used in Project 2 is a recursive pathfinding algorithm that utilizes memoization.
    When first called, the algorithm builds a routing table of the optimal ambulance and route for
    each possible location. This initialization explores the direct route (staging location -> call
    location) and the most viable alternative route. The heuristic in exploring alternative routes is that the most promising route will invalidate all subsequent ones (for the ambulance) whether
    valid or invalid. To avoid unnecessary processing, multiple early exits are implemented for when
    it's clear that the alternative route isn't viable in comparison to the direct route. The algorithm then processes subsequent ambulances, as appropriate, and updates the optimal route
    when applicable. Once initialization is complete, the results are stored as a function attribute
    for O(1) lookup on each call.
</p>
<p>
    In the application, this algorithm processes each location only on the first call. Once the
    routing table has been built, it processes each call as it comes in and provides O(1) lookup for
    each. This algorithm front-loads the heavy computations to achieve consistent speed in call
    processing.
</p>
<p>
    Time Complexity: O(n^3) - At worst, there are three levels of nested processing taking place.
</p>
<p>
    Space Complexity: O(n^2) - The routing table that's created stores the best route for each
    possible location. However, the path could be anywhere between one stop and the total number of
    possible stops long.
</p>
<p>
    There are two major advantages with this algorithm. First, we're guaranteeing that the optimal
    route will be provided for every call. Second, once the lookup table is built, each call is
    processed in a consistent amount of time due to O(1) lookup speed. In other words, this algorithm
    scales relatively well to increasing inputs.
</p>
<p>
    One of the glaring disadvantages for this algorithm is the high cost of front-loaded computation.
    As the number of possible destinations and routes increases, the computational cost and time
    will explode exponentially. One mitigating factor in this is the amount of "pruning" that occurs
    during processing. Another disadvantage is the memory cost of the lookup table. Again, as
    destinations and possible routes increase, the memory requirements will explode exponentially as
    well. There's also the very real possibility that we won't use some or many of the pre-calculated
    routes; if tonight's calls are all on one side of the city, then much of the front-loaded processing is essentially wasted.
</p>
</br>

<p>G. Conduct performance testing of Prototype One and Prototype Two</p>

#### Algorithm 1
<ol>
    <li>Runtime: 0.20305466651916504 seconds</li>
    <li>Runtime: 0.20597481727600098 seconds</li>
    <li>Runtime: 0.20253968238830566 seconds</li>
    <li>Runtime: 0.20573019981384277 seconds</li>
    <li>Runtime: 0.20605230331420898 seconds</li>
    <li>Runtime: 0.20773863792419434 seconds</li>
    <li>Runtime: 0.20897793769836426 seconds</li>
    <li>Runtime: 0.20707273483276367 seconds</li>
    <li>Runtime: 0.2051088809967041 seconds</li>
    <li>Runtime: 0.20361614227294922 seconds</li>
</ol>
<p>Average runtime: 0.2055866 seconds</p>

#### Algorithm 2
<ol>
    <li>Runtime: 0.07721400260925293 seconds</li>
    <li>Runtime: 0.06951045989990234 seconds</li>
    <li>Runtime: 0.06943893432617188 seconds</li>
    <li>Runtime: 0.06795430183410645 seconds</li>
    <li>Runtime: 0.07097434997558594 seconds</li>
    <li>Runtime: 0.06848669052124023 seconds</li>
    <li>Runtime: 0.06919670104980469 seconds</li>
    <li>Runtime: 0.07117438316345215 seconds</li>
    <li>Runtime: 0.06812095642089844 seconds</li>
    <li>Runtime: 0.06883931159973145 seconds</li>
</ol>
<p>Average runtime: 0.070091009 seconds</p>
</br>

<p>
    H. Compare the performance of both algorithms and provide one recommendation for improving each algorithm based on your interpretation of the performance testing.
</p>
<p>
    Upon reviewing each algorithm's performance, it's clear that Algorithm 2 is significantly faster than Algorithm 1. Quantified, Algorithm 2 is 2.93 times faster than Algorithm 1 on average.
    Despite the front-loaded processing cost in Algorithm 2, the resulting O(1) lookup is clearly
    worth that initialization cost. The driver for Algorithm 1's processing time lies in the locations network lookup. For each ambulance, the algorithm performs a query on the locations network for the exact staging location/call location combination. While that may sound like an O(1) operation, it's actually an O(n). In Pandas, the query operation scans the entire dataframe
    and evalues any statements against each.
</p>
<p>
    For Algorithm 1, my recommendation is to utilize memoization here as well. To address the O(n) lookup time, I would utilize a routing table here as well to change lookup time from O(n) to O(1) per ambulance. Specifically, I would go as far as using a simplified version of the routing table built in Algorithm 2. In this case, there would be much less complexity and no recursion due to the assumption that the direct route is always fastest.
</p>
<p>
    For Algorithm 2, my recommendation is to adapt the logic to be more dynamic. The algorithm is fast, and it is already dynamic. However, the initialization processes every possible destination
    regardless of if there's a call there. Especially in a live environment, it would make more sense
    to determine the fastest route for each location as it comes in. Once processed, the result would then be stored for later use. This would cut down on some of the resource costs while also minimizing processing time further. Essentially, we would trade some of the lightning fast lookups for reduced initialization overhead.
</p>