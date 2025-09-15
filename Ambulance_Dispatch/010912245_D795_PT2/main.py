import pandas as pd
import time

# Algorithm - Determine the fastest route to a call (required)
def fastest_route(call_location: str, ambulance_df: pd.DataFrame, locations_df: pd.DataFrame):
    
    def fastest_alt_route(start: str, visited: set=None):
        if visited is None:
            visited = set()
        visited.add(start)

        # Copy is necessary for the next code block
        potential_stops = locations_df.query(f'Start == "{start}"').copy()

        # 1. Unsorted, this algorithm probably processes routes "randomly"
        # 2. By sorting, we're essentially guaranteeing minimum recursions
        # 3. Moving to live data, this optimization will greatly reduce resource cost
        potential_stops['Total_Time'] = potential_stops['Travel_Time'] + potential_stops['Traffic_Delay']
        potential_stops.sort_values(by=['Total_Time'], ascending=[True], inplace=True)

        for stop in potential_stops.itertuples():
            if (start == ambulance.Staging_Location) & (stop.End == destination):
                return "None"
            elif stop.End in visited:
                continue
            elif stop.Total_Time >= direct_total_time:
                return "None"
            elif (stop.Total_Time < direct_total_time) & (stop.End == destination):
                route = {
                    'Path': f"{stop.Start};{stop.End}",
                    'Time': stop.Travel_Time + stop.Traffic_Delay
                }
                return route
            elif (stop.Total_Time < direct_total_time) & ((direct_total_time - stop.Total_Time) < 1.0):
                return "None"
            else:
                next_stop = fastest_alt_route(stop.End, visited)
                if next_stop == "None":
                    return "None"
                
                route = {
                    'Path': f"{stop.Start};{next_stop['Path']}",
                    'Time': stop.Total_Time + next_stop['Time']
                }
                return route

      
    routes = {}
    total_time = 9999
    result = pd.DataFrame({
            'Ambulance': [''],
            'Route':[''],
            'Time': [9999]
        })
    
    if not hasattr(fastest_route, 'route_lookup'):
        destinations = locations_df['Start'].unique()
        for destination in destinations:
            total_time = 9999

            for ambulance in ambulance_df.itertuples():
                if destination == ambulance.Staging_Location:
                    ambulance_num = ambulance.Ambulance_Number
                    route = ambulance.Staging_Location
                    total_time = 0
                    break
                else:
                    direct_route = locations_df.query(
                        f'Start == "{ambulance.Staging_Location}" & End == "{destination}"'
                        )

                    # Assigning variables to keep the code more vertical
                    direct_travel_time = direct_route['Travel_Time'].values[0]
                    direct_traffic_delay = direct_route['Traffic_Delay'].values[0]
                    direct_total_time = direct_travel_time + direct_traffic_delay

                    alt_route = fastest_alt_route(ambulance.Staging_Location)
                    if type(alt_route) == dict:
                        if (alt_route['Time'] < total_time) & (alt_route['Time'] < direct_total_time):
                            ambulance_num = ambulance.Ambulance_Number
                            route = alt_route['Path']
                            total_time = alt_route['Time']
                    
                    if direct_total_time < total_time:
                        ambulance_num = ambulance.Ambulance_Number
                        route = f"{direct_route['Start'].values[0]};{direct_route['End'].values[0]}"
                        total_time = direct_total_time

            routes[destination] = {
                'Ambulance': ambulance_num, 
                'Route': route, 
                'Total_Time': total_time
                }  
             
        fastest_route.route_lookup = routes

    result['Ambulance'] = fastest_route.route_lookup[call_location]['Ambulance']
    result['Route'] = fastest_route.route_lookup[call_location]['Route']
    result['Time'] = fastest_route.route_lookup[call_location]['Total_Time']
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