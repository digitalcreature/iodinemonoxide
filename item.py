class Item(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(80))
    image = db.Column(db.LargeBinary)
    timeStamp = db.Column(db.DateTime(timezone=False))

    def __init__(self, name, image, timeStamp):
        self.name = name
        self.image = image
        self.timeStamp = timeStamp

    def __repr__(self):
        return '<Name %r>' % self.name
