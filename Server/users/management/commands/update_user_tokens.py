from django.core.management.base import BaseCommand
from users.models import CustomUser
import string
import secrets

class Command(BaseCommand):
    help = 'Updates tokens for all existing users to be exactly 20 characters long'

    def handle(self, *args, **kwargs):
        users = CustomUser.objects.all()
        alphabet = string.ascii_letters + string.digits
        
        for user in users:
            # Generate new 20-character token
            user.token = ''.join(secrets.choice(alphabet) for _ in range(20))
            user.save()
            self.stdout.write(self.style.SUCCESS(f'Updated token for user {user.username}: {user.token}')) 