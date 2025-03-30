from django.db import models
from django.conf import settings
from django.utils import timezone

class Media(models.Model):
    MEDIA_TYPES = [
        ('image', 'Image'),
        ('video', 'Video'),
    ]

    user = models.ForeignKey(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    title = models.CharField(max_length=100)
    user_token = models.CharField(max_length=20, null=True, blank=True)  # Allow null initially
    media_type = models.CharField(max_length=5, choices=MEDIA_TYPES)
    file = models.FileField(upload_to='user_media/')
    uploaded_at = models.DateTimeField(default=timezone.now)
    token_cost = models.IntegerField(default=1)  # 1 for image, 2 for video

    def __str__(self):
        return f"{self.title} - {self.user.username}"

    def save(self, *args, **kwargs):
        # Set token cost based on media type
        if self.media_type == 'video':
            self.token_cost = 2
        else:
            self.token_cost = 1
        # Store the user's token at upload time
        if not self.user_token:
            self.user_token = self.user.token
        super().save(*args, **kwargs)
