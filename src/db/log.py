from db.redis_pubsub import *
import inspect

def line_number():
    return inspect.currentframe().f_back.f_lineno

def print_log(in_mem_db, source, timestamp, module, actionType,  actionValue, message, line, level):
    log_message = f"source={source};timestamp={timestamp};module={module};actionType={actionType};actionValue={actionValue};message={message};line={line}"
    if level == "DEBUG":
        in_mem_db.connection.publish(Channel.Log.value, log_message)
    elif level == "INFO":
        in_mem_db.connection.publish(Channel.Log.value, log_message)