class person:
    def __init__(self, name, byear):
        self.name = name
        self.byear = byear

    def age(self):
        return 2026 - self.byear

def Add2 (x,y):
    return x+y

def age(p):
    return p.age()