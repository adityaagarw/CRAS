from enum import Enum

class Channel(Enum):
    Backend = "Backend"
    Billing = "Billing"
    Frontend = "Frontend"

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
    TempCustomer = "TempCustomer"
    DeleteCustomer = "DeleteCustomer"
    BillingCustomer = "BillingCustomer"
    Employee = "Employee"

class FrontendMessage(Enum):
    StartBilling = "StartBilling"
