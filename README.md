# Calculator
Calculator using strings which I made for fun.

Takes an example input "((8 + 5) * 6) / 2" and prints the correct answer to that equation. 

## 2026 UPDATE
I have now made an improved version of this 4 years later to test my new skills and see how far I've come!

Now features user input verification and "fixing". Also now allows for negative values and decimal values!
Release has two exes, debug shows all logs and release only shows errors.

### Example loop
```
                  ┌───────────────────────────────┐
                  │       Start Program           │
                  └─────────────┬─────────────────┘
                                │
                                ▼
                  ┌───────────────────────────────┐
                  │          VerifyInput          │
                  └─────────────┬─────────────────┘
                                │
                                ▼
                  ┌───────────────────────────────┐
                  │           MainLoop            │
                  └─────────────┬─────────────────┘
                                │
                                ▼
                  ┌───────────────────────────────┐
                  │ Look for brackets in input    │
                  └───────┬───────────────────┬───┘
                          │ Found             │ Not Found
                          ▼                   ▼
         ┌─────────────────────────────┐    ┌─────────────────────────┐
         │ Are there operations inside │    │ Look for operators in   │
         │ the brackets?               │    │ input                   │
         └───────┬─────────┬───────────┘    └───────┬─────────────────┘
                 │ Yes       │ No                   │ Found
                 ▼           ▼                      ▼
     ┌─────────────────┐  ┌─────────────────┐    ┌─────────────────────────┐
     │ Calculate first │  │ Remove empty    │    │ Calculate first operator│
     │ operation inside│  │ brackets        │    │ found                   │
     │ brackets        │  │                 │    └──────────┬──────────────┘
     └───────┬─────────┘  └───────┬─────────┘               │
             │                    │                         │
             ▼                    ▼                         ▼
      ┌───────────────┐   ┌─────────────────┐      ┌─────────────────────────┐
      │ Replace result├►  │ Loop back to    │      │ Replace result in input │
      │ in input      │   │ MainLoop        │      └────────────┬────────────┘
      └───────────────┘   └─────────────────┘                   │
                          ▲                                     ▼
                          │       ┌─────────────────────────────┘
                          │ Yes   │
                          │       ▼
                  ┌───────────────────────────────┐
                  │ Operators left?               │
                  └───────┬───────────────────────┘
                          │ No
                          ▼
                  ┌───────────────────────────────┐
                  │ Display final result          │
                  └───────────────────────────────┘
```
