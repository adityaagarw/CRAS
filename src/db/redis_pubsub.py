from enum import Enum

class Channel(Enum):
    Backend = "Backend"
    Billing = "Billing"
    Frontend = "Frontend"
    CancelTimer = "CancelTimer"
    Employee = "Employee"
    Log = "Log"

class Status(Enum):
    BackEndDown = "BackEndDown"
    BackEndUp = "BackEndUp"
    FrontEndDown = "FrontEndDown"
    FrontEndUp = "FrontEndUp"

class BackendStatus(Enum):
    EntryCamUp = "EntryCamUp"
    EntryCamDown = "EntryCamDown"
    BillingCamUp = "BillingCamUp"
    BillingInProcess = "BillingInProcess"
    BillingCamDown = "BillingCamDown"
    ExitCamUp = "ExitCamUp"
    ExitCamDown = "ExitCamDown"

class BackendMessage(Enum):
    NewCustomer = "NewCustomer"
    UpdateCustomer = "UpdateCustomer"
    ExitingCustomer = "ExitingCustomer"
    DeleteCustomer = "DeleteCustomer"
    BillingCustomer = "BillingCustomer"
    RescanCustomer = "RescanCustomer"
    Employee = "Employee"
    EndBilling = "EndBilling"
    EndRescan = "EndRescan"
    CancelTimer = "CancelTimer"
    CancelQueue = "CancelQueue"
    NewEmployeeAck = "NewEmployeeAck"
    MarkAsEmployeeAck = "MarkAsEmployeeAck"
    EmployeeExists = "EmployeeExists"

class FrontendMessage(Enum):
    StartBilling = "StartBilling"
    StartRescan = "StartRescan"
    NewEmployee = "NewEmployee"
    MarkAsEmployee = "MarkAsEmployee"
