import sys
from bardapi import Bard
from bardapi import BardCookies

if len(sys.argv) != 2:
    print("Usage: python main.py 'your_argument'")
    sys.exit(1)

argument = sys.argv[1]

cookie_dict = {
    %here%
}

bard = BardCookies(cookie_dict=cookie_dict)
print(bard.get_answer(argument)['content'])