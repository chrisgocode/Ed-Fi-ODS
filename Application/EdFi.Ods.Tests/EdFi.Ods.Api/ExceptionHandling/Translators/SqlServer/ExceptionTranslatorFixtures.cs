﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using EdFi.Ods.Api.ExceptionHandling;
using EdFi.Ods.Api.ExceptionHandling.Translators;
using EdFi.Ods.Api.ExceptionHandling.Translators.SqlServer;
using EdFi.Ods.Api.Models;
using EdFi.Ods.Api.Providers;
using EdFi.Ods.Common.Exceptions;
using EdFi.Ods.Tests._Builders;
using EdFi.TestFixture;
using FakeItEasy;
using NUnit.Framework;

namespace EdFi.Ods.Tests.EdFi.Ods.Common.ExceptionHandling
{
    public class When_an_insert_conflicts_with_a_foreign_key_constraint_with_a_single_column : TestFixtureBase
    {
        private Exception _suppliedInsertException;
        private IEdFiProblemDetails _actualError;

        protected override void Arrange()
        {
            _suppliedInsertException =
                NHibernateExceptionBuilder.CreateException(
                    "could not execute batch command.[SQL: SQL not available]",
                    $"The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_StudentAddress_AddressType_AddressTypeId\". The conflict occurred in database \"EdFi_Ods\", table \"edfi.AddressType\", column 'AddressTypeId'.{Environment.NewLine}The statement has been terminated.{Environment.NewLine}");
        }

        protected override void Act()
        {
            var translator = new SqlServerConstraintExceptionTranslator();
            translator.TryTranslate(_suppliedInsertException, out _actualError);
        }

        [Test]
        public virtual void Should_respond_with_a_409_Conflict()
        {
            Assert.That(_actualError.Status, Is.EqualTo((int) HttpStatusCode.Conflict));
            Assert.That(_actualError.Type, Is.EqualTo(string.Join(':', EdFiProblemDetailsExceptionBase.BaseTypePrefix, "data-conflict:unresolved-reference")));
        }

        [Test]
        public virtual void
            Should_translate_the_message_to_indicate_that_a_related_resource_does_not_have_the_value_specified_in_the_current_request_but_does_not_provide_column_level_details()
        {
            Assert.That(
                _actualError.Detail, Is.EqualTo("The referenced 'AddressType' item does not exist."));
        }
    }

    public class When_an_update_conflicts_with_a_foreign_key_constraint_with_a_single_column : TestFixtureBase
    {
        private Exception _suppliedUpdateException;
        private IEdFiProblemDetails _actualError;

        protected override void Arrange()
        {
            _suppliedUpdateException = NHibernateExceptionBuilder.CreateException(
                "could not update: [something-a-rather][SQL: SQL not available]",
                $"The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Student_LimitedEnglishProficiencyType_LimitedEnglishProficiencyTypeId\". The conflict occurred in database \"EdFi_Ods\", table \"edfi.LimitedEnglishProficiencyType\", column 'LimitedEnglishProficiencyTypeId'.{Environment.NewLine}The statement has been terminated.{Environment.NewLine}");
        }

        protected override void Act()
        {
            var translator = new SqlServerConstraintExceptionTranslator();
            translator.TryTranslate(_suppliedUpdateException, out _actualError);
        }

        [Test]
        public virtual void Should_respond_with_a_409_Conflict()
        {
            Assert.That(_actualError.Status, Is.EqualTo((int) HttpStatusCode.Conflict));
            Assert.That(_actualError.Type, Is.EqualTo(string.Join(':', EdFiProblemDetailsExceptionBase.BaseTypePrefix, "data-conflict:unresolved-reference")));
        }

        [Test]
        public virtual void
            Should_translate_the_message_to_indicate_that_a_related_resource_does_not_have_the_value_specified_in_the_current_request_but_does_not_provide_column_details()
        {
            Assert.That(
                _actualError.Detail,
                Is.EqualTo("The referenced 'LimitedEnglishProficiencyType' item does not exist."));
        }
    }

    public class When_a_delete_conflicts_with_a_reference_constraint_with_multiple_columns : TestFixtureBase
    {
        private Exception _suppliedUpdateException;
        private IEdFiProblemDetails _actualError;

        protected override void Arrange()
        {
            _suppliedUpdateException = NHibernateExceptionBuilder.CreateException(
                "could not delete: [something-a-rather][SQL: SQL not available]",
                $"The DELETE statement conflicted with the REFERENCE constraint \"FK_DisciplineAction_DisciplineIncident_SchoolId\". The conflict occurred in database \"EdFi_Ods\", table \"edfi.DisciplineAction\".{Environment.NewLine}The statement has been terminated.");
        }

        protected override void Act()
        {
            var translator = new SqlServerConstraintExceptionTranslator();
            translator.TryTranslate(_suppliedUpdateException, out _actualError);
        }

        [Test]
        public virtual void Should_respond_with_a_409_Conflict()
        {
            Assert.That(_actualError.Status, Is.EqualTo((int) HttpStatusCode.Conflict));
            Assert.That(_actualError.Type, Is.EqualTo(string.Join(':', EdFiProblemDetailsExceptionBase.BaseTypePrefix, "data-conflict:dependent-item-exists")));
        }

        [Test]
        public virtual void
            Should_translate_the_message_to_indicate_that_a_related_resource_does_not_have_the_value_specified_in_the_current_request_but_does_not_provide_column_details()
        {
            Assert.That(
                _actualError.Detail,
                Is.EqualTo(
                    "The requested action cannot be performed because this item is referenced by an existing 'DisciplineAction' item."));
        }
    }

    public class When_a_delete_conflicts_with_a_reference_constraint_with_a_single_column : TestFixtureBase
    {
        private Exception _suppliedUpdateException;
        private IEdFiProblemDetails _actualError;

        protected override void Arrange()
        {
            _suppliedUpdateException = NHibernateExceptionBuilder.CreateException(
                "could not delete: [something-a-rather][SQL: SQL not available]",
                $"The DELETE statement conflicted with the REFERENCE constraint \"FK_CourseTranscript_CourseAttemptResultType_CourseAttemptResultTypeId\". The conflict occurred in database \"EdFi_Ods\", table \"edfi.CourseTranscript\", column 'CourseAttemptResultTypeId'.{Environment.NewLine}The statement has been terminated.");
        }

        protected override void Act()
        {
            var translator = new SqlServerConstraintExceptionTranslator();
            translator.TryTranslate(_suppliedUpdateException, out _actualError);
        }

        [Test]
        public virtual void Should_respond_with_a_409_Conflict()
        {
            Assert.That(_actualError.Status, Is.EqualTo((int) HttpStatusCode.Conflict));
            Assert.That(_actualError.Type, Is.EqualTo(string.Join(':', EdFiProblemDetailsExceptionBase.BaseTypePrefix, "data-conflict:dependent-item-exists")));
        }

        [Test]
        public virtual void
            Should_translate_the_message_to_indicate_that_a_related_resource_does_not_have_the_value_specified_in_the_current_request_and_provide_column_details()
        {
            Assert.That(
                _actualError.Detail,
                Is.EqualTo(
                    "The requested action cannot be performed because this item is referenced by an existing 'CourseTranscript' item."));
        }
    }

    public class When_an_insert_conflicts_with_a_non_primary_key_unique_index_with_a_single_column : TestFixtureBase
    {
        private Exception _suppliedUpdateException;
        private IDatabaseMetadataProvider _suppliedMetadataProvider;

        private IEdFiProblemDetails _actualError;

        protected override void Arrange()
        {
            _suppliedUpdateException = NHibernateExceptionBuilder.CreateException(
                "could not insert: [EdFi.Ods.Entities.AcademicHonorsTypeAggregate.AcademicHonorsType][SQL: INSERT INTO edfi.AcademicHonorsType (LastModifiedDate, CreateDate, Id, CodeValue, Description) VALUES (?, ?, ?, ?, ?); select SCOPE_IDENTITY()]",
                $"Cannot insert duplicate key row in object 'edfi.AcademicHonorsType' with unique index 'SomeIndexName'.{Environment.NewLine}The statement has been terminated.");

            _suppliedMetadataProvider = A.Fake<IDatabaseMetadataProvider>();

            A.CallTo(() => _suppliedMetadataProvider.GetIndexDetails("SomeIndexName"))
                .Returns(
                    new IndexDetails
                    {
                        IndexName = "SomeIndexName",
                        TableName = "SomeTableName",
                        ColumnNames = new List<string> {"Column1"}
                    });
        }

        protected override void Act()
        {
            var translator = new SqlServerUniqueIndexExceptionTranslator(_suppliedMetadataProvider);
            translator.TryTranslate(_suppliedUpdateException, out _actualError);
        }

        [Test]
        public virtual void Should_respond_with_a_409_Conflict()
        {
            Assert.That(_actualError.Status, Is.EqualTo((int) HttpStatusCode.Conflict));
            Assert.That(_actualError.Type, Is.EqualTo(string.Join(':', EdFiProblemDetailsExceptionBase.BaseTypePrefix, "data-conflict:non-unique-values")));
        }

        [Test]
        public virtual void Should_translate_message_to_indicate_which_single_property_on_which_entity_was_not_unique()
        {
            Assert.That(
                _actualError.Detail,
                Is.EqualTo("The value unknown supplied for property 'column1' of entity 'someTableName' is not unique."));
        }
    }

    public class When_an_insert_conflicts_with_a_non_primary_key_unique_index_with_multiple_columns : TestFixtureBase
    {
        private Exception _suppliedUpdateException;
        private IDatabaseMetadataProvider _suppliedMetadataProvider;
        private IEdFiProblemDetails _actualError;

        protected override void Arrange()
        {
            _suppliedUpdateException = NHibernateExceptionBuilder.CreateException(
                "could not insert: [EdFi.Ods.Entities.AcademicHonorsTypeAggregate.AcademicHonorsType][SQL: INSERT INTO edfi.AcademicHonorsType (LastModifiedDate, CreateDate, Id, CodeValue, Description) VALUES (?, ?, ?, ?, ?); select SCOPE_IDENTITY()]",
                $"Cannot insert duplicate key row in object 'edfi.AcademicHonorsType' with unique index 'SomeIndexName'.{Environment.NewLine}The statement has been terminated.");

            _suppliedMetadataProvider = A.Fake<IDatabaseMetadataProvider>();

            A.CallTo(() => _suppliedMetadataProvider.GetIndexDetails("SomeIndexName"))
                .Returns(
                    new IndexDetails
                    {
                        IndexName = "SomeIndexName",
                        TableName = "SomeTableName",
                        ColumnNames = new List<string>
                        {
                            "Column1",
                            "Column2"
                        }
                    });
        }

        protected override void Act()
        {
            var translator = new SqlServerUniqueIndexExceptionTranslator(_suppliedMetadataProvider);
            translator.TryTranslate(_suppliedUpdateException, out _actualError);
        }

        [Test]
        public virtual void Should_respond_with_a_409_Conflict()
        {
            Assert.That(_actualError.Status, Is.EqualTo((int) HttpStatusCode.Conflict));
            Assert.That(_actualError.Type, Is.EqualTo(string.Join(':', EdFiProblemDetailsExceptionBase.BaseTypePrefix, "data-conflict:non-unique-values")));
        }

        [Test]
        public virtual void Should_translate_message_to_indicate_which_properties_on_which_entity_were_not_unique()
        {
            Assert.That(
                _actualError.Detail,
                Is.EqualTo(
                    "The values unknown supplied for properties 'column1', 'column2' of entity 'someTableName' are not unique."));
        }
    }

    public class When_an_insert_conflicts_with_a_non_primary_key_unique_index_with_a_single_column_and_values : TestFixtureBase
    {
        private Exception _suppliedUpdateException;
        private IDatabaseMetadataProvider _suppliedMetadataProvider;

        private IEdFiProblemDetails _actualError;

        protected override void Arrange()
        {
            _suppliedUpdateException = NHibernateExceptionBuilder.CreateException(
                "could not insert: [EdFi.Ods.Entities.AcademicHonorsTypeAggregate.AcademicHonorsType][SQL: INSERT INTO edfi.AcademicHonorsType (LastModifiedDate, CreateDate, Id, CodeValue, Description) VALUES (?, ?, ?, ?, ?); select SCOPE_IDENTITY()]",
                $"Cannot insert duplicate key row in object 'edfi.AcademicHonorsType' with unique index 'SomeIndexName'. The duplicate key value is (69).{Environment.NewLine}The statement has been terminated.");

            _suppliedMetadataProvider = A.Fake<IDatabaseMetadataProvider>();

            A.CallTo(() => _suppliedMetadataProvider.GetIndexDetails("SomeIndexName"))
                .Returns(
                    new IndexDetails
                    {
                        IndexName = "SomeIndexName",
                        TableName = "SomeTableName",
                        ColumnNames = new List<string> {"Column1"}
                    });
        }

        protected override void Act()
        {
            var translator = new SqlServerUniqueIndexExceptionTranslator(_suppliedMetadataProvider);
            translator.TryTranslate(_suppliedUpdateException, out _actualError);
        }

        [Test]
        public virtual void Should_respond_with_a_409_Conflict()
        {
            Assert.That(_actualError.Status, Is.EqualTo((int) HttpStatusCode.Conflict));
            Assert.That(_actualError.Type, Is.EqualTo(string.Join(':', EdFiProblemDetailsExceptionBase.BaseTypePrefix, "data-conflict:non-unique-values")));
        }

        [Test]
        public virtual void Should_translate_message_to_indicate_which_single_property_on_which_entity_was_not_unique()
        {
            Assert.That(
                _actualError.Detail,
                Is.EqualTo("The value (69) supplied for property 'column1' of entity 'someTableName' is not unique."));
        }
    }

    [TestFixture]
    public class When_an_ArgumentException_is_thrown : TestFixtureBase
    {
        private Exception _suppliedException;
        private IEdFiProblemDetails _actualError;
        private bool _result;

        protected override void Arrange()
        {
            _suppliedException = new ArgumentException("Some error message");
        }

        protected override void Act()
        {
            var translator = new ValidationExceptionTranslator();
            _result = translator.TryTranslate(_suppliedException, out _actualError);
        }

        [Test]
        public virtual void Should_be_unable_to_translate_error()
        {
            Assert.That(_result, Is.False);
            Assert.That(_actualError, Is.Null);
        }
    }
}