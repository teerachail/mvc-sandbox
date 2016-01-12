param([int] $iterations = 50000)

$url = "http://127.0.0.1:5000/"

& loadtest -k -n $iterations -c 25 $url