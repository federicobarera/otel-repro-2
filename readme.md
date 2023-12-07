# Repro Steps

1. Start the project
2. Verify it runs on http://localhost:5216/weatherforecast or adjusts ports for later steps
3. `curl http://localhost:5216/weatherforecast`
4. Verify in console that a trace has been output
5. Send request with `traceparent` header and verify that no traces have been created:
    ```
    curl  -X GET \
    'http://localhost:5216/weatherforecast' \
    --header 'Accept: */*' \
    --header 'User-Agent: Thunder Client (https://www.thunderclient.com)' \
    --header 'traceparent: 00-4ce102de67628f2f0602e4af01ec45ec-622d5b6fc414deb4-00'
    ```