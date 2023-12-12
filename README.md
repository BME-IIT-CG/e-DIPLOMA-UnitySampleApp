# e-DIPLOMA-UnitySampleApp

 A minimal demonstration of how Unity Photon Fusion and Amazon GameLift work together.

 ## Requirements for runnning

 - Unity
 - Photon registration and Fusion App ID
 - AWS CLI tool
 - Client and headless build

 ## Building

 1. Clone repository
 2. Open project in Unity
 3. Add your own App ID in Fusion settings
 4. Go to build settings
 5. Choose desktop as platform, and build
 6. Repeat with dedicated server

 ## Running

 1. Launch local GameLift instance like this: `java - jar ./ GameLiftLocal . jar`
 2. Launch headless Unity server binary: `./GameLiftPhotonFusionDemo.exe -ServerPort PORT`
 3. Acquire GameLift session ID using the AWS tool
     1. Set AWS environment variables:
         ```
         $env:AWS_DEFAULT_REGION = 'eu-central-1'
         $env:AWS_ACCESS_KEY_ID = 'foobar'
         $env:AWS_SECRET_ACCESS_KEY = 'foobar'
         ```
     2. Create a game session: `aws gamelift create-game-session --endpoint-url http://localhost:8080 --fleet -id your-fleet --maximum-player-session-count 4`
     3. Create a player session using the game session ID: `aws gamelift create-player-session --endpoint-url http://localhost:8080 --game-session-id SESSION_ID --player-id PLAYER_ID --player-data PLAYER_NAME`
 4. Launch Unity client with the acquired game session ID: `./GameLiftPhotonFusionDemo.exe -GLID SESSION_ID`
 5. Launch another client with the same ID (for now)
            
![Screenshot 2023-12-12 171315](https://github.com/BME-IIT-CG/e-DIPLOMA-UnitySampleApp/assets/71231495/93faa6de-0177-40ff-acab-bfeefea64ee9)