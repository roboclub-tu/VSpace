from django.contrib.auth.models import AbstractUser
from django.db import models
import secrets
import string

class CustomUser(AbstractUser):
    ROLE_CHOICES = [
        ('researcher', 'Researcher'),
        ('participant', 'Participant'),
    ]
    
    role = models.CharField(max_length=20, choices=ROLE_CHOICES, default='participant')
    email = models.EmailField(unique=True)
    token = models.CharField(max_length=20, unique=True, blank=True, null=True)
    tokens_amount = models.IntegerField(default=0, help_text="Amount of special currency tokens")

    def save(self, *args, **kwargs):
        if not self.token:
            # Generate a 20-character token using letters and numbers
            alphabet = string.ascii_letters + string.digits
            self.token = ''.join(secrets.choice(alphabet) for _ in range(20))
        super().save(*args, **kwargs)

    def __str__(self):
        return self.username
