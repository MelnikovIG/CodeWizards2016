SET local_runner_folder=..\local-runner\
SET exe_path=..\csharp-cgdk\bin\debug\

start javaw -Xms512m -Xmx1G -XX:+UseConcMarkSweepGC -jar "%local_runner_folder%local-runner.jar" local-runner.properties Configs/2x5.properties
timeout 1

FOR /L %%A IN (1,1,5) DO (
  start /b %exe_path%csharp-cgdk.exe "127.0.0.1" "3100%%A " "0000000000000000"
  timeout 1
)