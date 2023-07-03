from datetime import datetime
from datetime import timedelta
import time

# timestamp1 = datetime.strptime("2023-06-10 22:49:41.793134", "%Y-%m-%d %H:%M:%S.%f")
# timestamp2 = datetime.strptime("2023-06-10 23:51:43.790000", "%Y-%m-%d %H:%M:%S.%f")

# timestamp3 = datetime.strptime("2023-06-10 22:40:41.793134", "%Y-%m-%d %H:%M:%S.%f")
# timestamp4 = datetime.strptime("2023-06-10 22:58:43.790000", "%Y-%m-%d %H:%M:%S.%f")

# interval1 = timestamp2 - timestamp1
# interval2 = timestamp4 - timestamp3

# str_i1 = str(interval1)
# str_i2 = str(interval2)

# intervaltime1 = datetime.strptime(str_i1, "%H:%M:%S.%f")
# intervaltime2 = datetime.strptime(str_i2, "%H:%M:%S.%f")

# intervaltime1_timedelta = intervaltime1 - datetime(1900, 1, 1)
# intervaltime1_seconds = intervaltime1_timedelta.total_seconds()

# print("Intervaltime1 in seconds:", intervaltime1_timedelta)
# #print(intervaltime1 * 2)

# #print(str_i1)
#print(str_i2)


date_format = "%Y-%m-%d %H:%M:%S"
timestamp1 = datetime.strptime(datetime.now().strftime(date_format), date_format)
time.sleep(5)
timestamp2 = datetime.strptime(datetime.now().strftime(date_format), date_format)

print("T1: ", timestamp1, " T2: ", timestamp2)

interval = timestamp2 - timestamp1
abc =timedelta.total_seconds(interval)

#interval_datetime = datetime.strptime(interval, date_format)

print("interval: ", interval.total_seconds())



