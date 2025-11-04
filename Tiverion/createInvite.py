import sqlite3
import uuid
from datetime import datetime

db_file = "vespa.db"

conn = sqlite3.connect(db_file)
cursor = conn.cursor()

invitations_to_add = [
    {
        "CreatedByUserId": 1,
        "Title": "Приглашение для теста",
        "Message": "Используйте этот код для регистрации",
        "Role": "User"
    },
    {
        "CreatedByUserId": 1,
        "Title": "Админское приглашение",
        "Message": "Для регистрации администратора",
        "Role": "Admin"
    }
]

for inv in invitations_to_add:
    inv_id = str(uuid.uuid4())
    print(inv_id)
    created_at = datetime.utcnow().isoformat()
    cursor.execute("""
        INSERT INTO Invitations (Id, CreatedByUserId, ActivatedAt, CreatedAt, ExpiresAt, Role, Title, Message)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
    """, (
        inv_id,
        inv["CreatedByUserId"],
        None,
        created_at,
        None,
        inv["Role"],
        inv["Title"],
        inv["Message"]
    ))

conn.commit()
conn.close()
print(f"{len(invitations_to_add)} приглашений добавлено в базу.")
