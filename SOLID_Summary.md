### 1 S — Single Responsibility Principle (SRP)
* What does it mean? A class should have one, and only one, reason to change.
* Example from IceCity project: CostService is only responsible for coordinating the cost calculation.
* What happens when SRP is violated? The class becomes too complex. Changing one feature might break another.

### 2 O — Open/Closed Principle (OCP)
* What does it mean? Software entities should be open for extension, but closed for modification.
* Why is it important? We added SolarHeater without changing the existing CostService logic.

### 3 L — Liskov Substitution Principle (LSP)
* What does it mean? Objects of a superclass should be replaceable with objects of its subclasses.
* Simple example: SolarHeater perfectly replaces HeaterBase because it correctly implements GetEffectivePower().

### 4 I — Interface Segregation Principle (ISP)
* What does it mean? No client should be forced to depend on methods it does not use.
* Example: ICostCalculationStrategy only has calculation methods.

### 5 D — Dependency Inversion Principle (DIP)
* What does it mean? High-level modules should not depend on low-level modules. Both should depend on abstractions.
* How does ASP.NET Core DI container help? It allows us to inject ICostStrategyFactory into CostService via the constructor.
