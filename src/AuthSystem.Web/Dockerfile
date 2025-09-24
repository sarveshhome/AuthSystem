FROM node:20.11.1-alpine AS build
WORKDIR /app

# Copy package files from the correct location
COPY src/AuthSystem.Web/package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY src/AuthSystem.Web/ ./

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
