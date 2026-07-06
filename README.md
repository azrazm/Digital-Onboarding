to run the project, execute following: 

dotnet run 

depending which service you want: 

dotnet run \
  --project services/MerchantService/MerchantService.Api \
  --urls "http://localhost:5000"

dotnet run \
  --project services/ProductService/ProductService.Api \
  --urls "http://localhost:5001"

dotnet run \
  --project services/OnboardingApplicationService/OnboardingApplicationService.Api \
  --urls "http://localhost:5002"

dotnet run \
  --project services/DocumentService/DocumentService.Api \
  --urls "http://localhost:5003"
