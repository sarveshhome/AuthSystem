#!/bin/bash

echo "Running all unit tests for AuthSystem..."
echo "========================================"

# Navigate to the tests directory
cd "$(dirname "$0")"

echo "1. Running Application Unit Tests..."
cd AuthSystem.Application.UnitTests
dotnet test --verbosity normal --collect:"XPlat Code Coverage"
echo ""

echo "2. Running API Unit Tests..."
cd ../AuthSystem.Api.UnitTests
dotnet test --verbosity normal --collect:"XPlat Code Coverage"
echo ""

echo "3. Running Integration Tests..."
cd ../AuthSystem.Api.IntegrationTests
dotnet test --verbosity normal --collect:"XPlat Code Coverage"
echo ""

echo "All tests completed!"
echo "===================="