param([int] $iterations = 3000)

$url = "http://127.0.0.1:5000/pet"

& ab -k -n $iterations -c 16 -T application/json -p post_pet.txt $url