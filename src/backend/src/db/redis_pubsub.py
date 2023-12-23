from enum import Enum

class Channel(Enum):
    Backend = "Backend"
    Billing = "Billing"
    Frontend = "Frontend"
    CancelTimer = "CancelTimer"
    Employee = "Employee"
    Log = "Log"
    Status = "Status"

class Status(Enum):
    BackendDown = "BackendDown"
    BackendUp = "BackendUp"
    FrontendDown = "FrontendDown"
    FrontendUp = "FrontendUp"
    EntryCamDown = "EntryCamDown"
    EntryCamUp = "EntryCamUp"
    BillingCamDown = "BillingCamDown"
    BillingCamUp = "BillingCamUp"
    ExitCamDown = "ExitCamDown"
    ExitCamUp = "ExitCamUp"

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
    EmployeeEntered = "EmployeeEntered"
    EmployeeExited = "EmployeeExited"

class FrontendMessage(Enum):
    StartBilling = "StartBilling"
    StartRescan = "StartRescan"
    NewEmployee = "NewEmployee"
    MarkAsEmployee = "MarkAsEmployee"
