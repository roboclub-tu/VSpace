from django.core.management.base import BaseCommand
from users.models import CustomUser

class Command(BaseCommand):
    help = 'Gives 10 tokens to all users'

    def handle(self, *args, **options):
        users = CustomUser.objects.all()
        for user in users:
            user.tokens_amount = 10
            user.save()
        
        self.stdout.write(
            self.style.SUCCESS(f'Successfully gave 10 tokens to {users.count()} users')
        ) 