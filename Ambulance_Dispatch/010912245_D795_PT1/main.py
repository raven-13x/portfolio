import pandas as pd
import time

# Algorithm - Determine the fastest route to a call (required)
def fastest_route(call_location: str, ambulance_df: pd.DataFrame, locations_df: pd.DataFrame):
    early_exit = False
    result = pd.DataFrame({
            'Ambulance': [''],
            'Route':[''],
            'Time': [9999]
        })
    
    for ambulance in ambulance_df.itertuples():
        if early_exit:
            return result

        ambulance_num = ambulance.Ambulance_Number

        if ambulance.Staging_Location == call_location:
            route = f"{ambulance.Staging_Location}"
            total_time = 0
            early_exit = True
        else:
            location = locations_df.query(
                f'Start == "{ambulance.Staging_Location}" & End == "{call_location}"')
            
            route = f"{ambulance.Staging_Location};{call_location}"
            total_time = location['Travel_Time'].iloc[0] + location['Traffic_Delay'].iloc[0]

        if result['Ambulance'].values[0] == '' or result['Time'].values[0] > total_time:
            result['Ambulance'] = ambulance_num
            result['Route'] = route
            result['Time'] = total_time
    
    return result
        

def main():
    pd.options.display.max_rows = 9999

    calls_df = pd.read_csv('Test_Data/calls.csv')
    call_prty_df = pd.read_csv('Test_Data/call_priority.csv')
    ambulance_df = pd.read_csv('Test_Data/ambulance.csv')
    locations_df = pd.read_csv('Test_Data/location_network.csv')
    record_df = pd.DataFrame(columns=['Record'])
    total_time = 0

    # Sort calls by priority then by order received (required)
    # Assuming all datasets were cleaned prior to use in this program
    prioritized_calls_df = pd.merge(left=calls_df, right=call_prty_df, how="inner", on="Call Type")
    prioritized_calls_df.sort_values(
        by=["Priority", "Call ID"], ascending=[True, True], inplace=True
        )

    # Tuple attributes cannot have spaces
    prioritized_calls_df.rename(columns={
        "Call ID":"Call_ID", 
        "Call Type":"Call_Type"}, 
        inplace=True)
    ambulance_df.rename(columns={
        "Ambulance Number":"Ambulance_Number", 
        "Staging Location":"Staging_Location"},
        inplace=True)
    locations_df.rename(columns={
        "Travel Time":"Travel_Time", 
        "Traffic Delay":"Traffic_Delay"},
        inplace=True)

    # Process call list, determine fastest dispatch, create record, and add to log (required)
    for call in prioritized_calls_df.itertuples():

        # Start embedded timer for performance metrics (required)
        start_time = time.time()

        assignment = fastest_route(call.Location, ambulance_df, locations_df)

        # End embedded timer for performance metrics (required)
        end_time = time.time()

        total_time += (end_time - start_time)

        dispatch_record = (f"Call_ID={call.Call_ID},"
                           f"Call_Type={call.Call_Type},"
                           f"Call_Location={call.Location},"
                           f"Selected_Ambulance={assignment['Ambulance'].values[0]},"
                           f"Route_to_Call_Location={assignment['Route'].values[0]},"
                           f"Time_to_Call_Location={assignment['Time'].values[0]}"
                           )
        record_df = pd.concat([record_df, pd.DataFrame([dispatch_record])], ignore_index=True)
    
    # Single write operation is more efficient than multiple appends
    record_df.to_csv('log/ambulance_call_log.csv')

    print(f"Total algorithm runtime: {total_time} seconds")

if __name__ == "__main__":
    main()